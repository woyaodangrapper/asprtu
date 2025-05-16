using Asprtu.Core.Interfaces;
using Asprtu.Core.Protocol;
using Asprtu.Repository.MemoryCache;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities.EventHub.Discovery;

// 被动探测
public class PassiveDiscoveryWorker : BackgroundService, IDiscovery
{
    private readonly IQueueFactory _queueFactory;

    private readonly QueueOptions _exporterOptions = new(
        queueName: "blackbox_exporter_registration",
        capacity: 1024 * 1024);

    private readonly QueueOptions _importerOptions = new(
        queueName: "blackbox_importer_registration",
        capacity: 1024 * 1024);

    private readonly long _userSecretsId = SnowflakeId.NewSnowflakeId();

    private readonly ILogger<PassiveDiscoveryWorker> _logger;

    public PassiveDiscoveryWorker(
        IQueueFactory queueFactory,
        ILogger<PassiveDiscoveryWorker> logger
        )
    {
        _queueFactory = queueFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 发送握手消息
        var queueOptions = await ReceiveLoop(stoppingToken).WaitAsync(TimeSpan.FromSeconds(30), stoppingToken);

        // 心跳定时发送
        var heartbeatTask = Task.Run(() => HeartbeatLoop(queueOptions, stoppingToken), stoppingToken);

        await Task.WhenAll(heartbeatTask);
    }

    /// <summary>
    /// Continuously listens for messages from a subscriber and attempts to establish a handshake with a publisher. If a
    /// valid handshake is detected, returns the queue options for further processing.
    /// </summary>
    /// <remarks>This method operates in a loop, publishing heartbeat messages and waiting for a response from
    /// the subscriber. If a valid handshake is detected, the method returns the corresponding queue options. The loop
    /// continues until the provided <paramref name="token"/> signals cancellation or an exception occurs. Exceptions
    /// are handled internally, and the loop will retry after a delay unless cancellation is requested.</remarks>
    /// <param name="token">A <see cref="CancellationToken"/> used to signal cancellation of the receive loop.</param>
    /// <returns>A <see cref="QueueOptions"/> object containing the queue name and capacity if a valid handshake is established;
    /// otherwise, <see langword="null"/> if the operation is canceled or no handshake is completed.</returns>
    private async Task<QueueOptions?> ReceiveLoop(CancellationToken token)
    {
        using IPublisher publisher = _queueFactory.CreatePublisher(_importerOptions);
        using ISubscriber subscriber = _queueFactory.CreateSubscriber(_exporterOptions);

        while (!token.IsCancellationRequested)
        {
            try
            {
                HeartHandles heartHandles = new HeartHandles(userSecretsId: _userSecretsId)
                 .ToBytes(out byte[]? bytes);
                _ = publisher.TryEnqueue(bytes);

                // 等待握手
                if (subscriber.Dequeue(token) is { } message)
                {
                    HeartHandles register = new(message);
                    if (register.UserSecretsId == heartHandles.UserSecretsId)
                    {
                        return new(
                               queueName: register.HeartbeatName.ToString(),
                               capacity: 1024 * 1024);
                    }
                }
                await Task.Delay(1000, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                await Task.Delay(1000, token);
            }
        }
        return null;
    }

    /// <summary>
    /// Executes a continuous loop to send heartbeat messages to a queue at regular intervals.
    /// </summary>
    /// <remarks>This method sends heartbeat messages every 5 seconds to the specified queue using the
    /// provided  <paramref name="option"/>. If the <paramref name="token"/> is canceled, the loop will terminate
    /// gracefully.</remarks>
    /// <param name="option">The configuration options for the queue. Must not be <see langword="null"/>.</param>
    /// <param name="token">A cancellation token that can be used to stop the loop.</param>
    /// <returns></returns>
    private async Task HeartbeatLoop(QueueOptions? option, CancellationToken token)
    {
        if (option == null)
        {
            _logger.LogError("心跳发送失败,握手失败!");
            return;
        }
        using IPublisher publisher = _queueFactory.CreatePublisher(option);

        while (!token.IsCancellationRequested)
        {
            try
            {
                _ = new HeartBeating(userSecretsId: _userSecretsId)
                .ToBytes(out byte[]? bytes);
                _ = publisher.TryEnqueue(bytes);

                // 每隔 5 秒发送心跳
                await Task.Delay(TimeSpan.FromSeconds(5), token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                await Task.Delay(1000, token);
            }
        }
    }
}