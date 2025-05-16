using Asprtu.Core.Interfaces;
using Asprtu.Core.Protocol;
using Asprtu.Repository.MemoryCache;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Asprtu.Capacities.EventHub.Discovery;

// 主动探测 使用 握手(1),心跳(N) 的内存结构
public class ActiveDiscoveryWorker : BackgroundService, IDiscovery
{
    private readonly IQueueFactory _queueFactory;

    private readonly QueueOptions _exporterOptions = new(
        queueName: "blackbox_exporter_registration",
        capacity: 1024 * 1024);

    private readonly QueueOptions _importerOptions = new(
       queueName: "blackbox_importer_registration",
       capacity: 1024 * 1024);

    private readonly ILogger<ActiveDiscoveryWorker> _logger;

    private readonly ConcurrentDictionary<ulong, HeartbeatContext>
        _dictionary = new();

    public ActiveDiscoveryWorker(
        IQueueFactory queueFactory,
        ILogger<ActiveDiscoveryWorker> logger
        )
    {
        _queueFactory = queueFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 接收通道消息
        var receivingTask = Task.Run(() => ReceiveLoop(stoppingToken), stoppingToken);

        // 心跳定时发送
        var heartbeatTask = Task.Run(() => HeartbeatLoop(stoppingToken), stoppingToken);

        // 超时清除线程
        var cleanupTask = Task.Run(() => CleanupLoopAsync(stoppingToken), stoppingToken);

        await Task.WhenAll(receivingTask, heartbeatTask, cleanupTask);
    }

    /// <summary>
    /// Continuously processes messages from a subscriber and publishes heartbeat messages to a queue.
    /// </summary>
    /// <remarks>This method listens for messages from a subscriber and processes them in a loop until the
    /// provided  <paramref name="token"/> signals cancellation. For each received message, it creates a unique queue
    /// for heartbeat messages and publishes a corresponding heartbeat message. If no message is available,  the method
    /// waits for a short delay before retrying.  Exceptions are logged if errors occur during message processing, but
    /// the loop continues unless  explicitly canceled.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the loop.</param>
    /// <returns></returns>
    private async Task ReceiveLoop(CancellationToken token)
    {
        using IPublisher publisher = _queueFactory.CreatePublisher(_exporterOptions);
        using ISubscriber subscriber = _queueFactory.CreateSubscriber(_importerOptions);

        while (!token.IsCancellationRequested)
        {
            try
            {
                // 接收握手包并发送握手包心跳偏移量
                if (subscriber.Dequeue(token) is { } message)
                {
                    HeartHandles register = new(message);

                    string queueName = $"blackbox_heart_beating_{Math.Abs(register.GetHashCode())}_{register.UserSecretsId}";

                    QueueOptions queueOptions = new(queueName: queueName, capacity: 1024 * 1024);
                    _ = _dictionary.TryAdd(register.UserSecretsId, new HeartbeatContext(HeartHandles: register, Option: queueOptions, Subscriber: _queueFactory.CreateSubscriber(queueOptions)));

                    _ = new HeartHandles(heartbeatName: queueName, userSecretsId: (long)register.UserSecretsId)
                         .ToBytes(out byte[]? bytes);
                    _ = publisher.TryEnqueue(bytes);
                }
                else
                {
                    await Task.Delay(500, token);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing discovery message.");
            }
        }
    }

    /// <summary>
    /// Periodically processes messages from subscribers and updates their associated timestamps.
    /// </summary>
    /// <remarks>This method continuously iterates through the keys in the internal dictionary and attempts to
    /// dequeue messages from each subscriber. If a message is successfully dequeued, the associated  timestamp is
    /// updated. The loop pauses for 5 seconds between iterations or exits if the  <paramref name="token"/> is
    /// canceled.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> used to signal the termination of the loop.</param>
    /// <returns></returns>
    private async Task HeartbeatLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (ulong key in _dictionary.Keys)
            {
                if (_dictionary.TryGetValue(key, out var context))
                {
                    if (context.Subscriber.TryDequeue(token, out var message))
                    {
                        HeartBeating register = new(message);
                        context.Timestamp = (long)register.Timestamp;
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5), token);
        }
    }

    /// <summary>
    /// Periodically removes expired entries from the internal dictionary until the operation is canceled.
    /// </summary>
    /// <remarks>This method continuously checks for expired entries in the dictionary and removes them based
    /// on a predefined timeout. The loop runs until the provided <paramref name="token"/> signals cancellation. The
    /// method handles exceptions gracefully, logging any errors that occur during the cleanup process.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> used to signal the cancellation of the cleanup loop.</param>
    /// <returns></returns>
    private async Task CleanupLoopAsync(CancellationToken token)
    {
        double _timeout = TimeSpan.FromSeconds(10).TotalMilliseconds;
        while (!token.IsCancellationRequested)
        {
            try
            {
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                IEnumerable<KeyValuePair<ulong, HeartbeatContext>> expiredKey = _dictionary.Where(
                    context => now - context.Value.Timestamp > _timeout
                );

                foreach (KeyValuePair<ulong, HeartbeatContext> item in expiredKey)
                {
                    _ = _dictionary.TryRemove(item);
                }

                await Task.Delay(TimeSpan.FromSeconds(5), token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清理心跳时发生错误");
            }
        }
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                foreach (HeartbeatContext context in _dictionary.Values)
                {
                    if (context.Subscriber is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                _dictionary.Clear();
            }
            // 释放非托管资源

            _disposed = true;
        }
    }

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}