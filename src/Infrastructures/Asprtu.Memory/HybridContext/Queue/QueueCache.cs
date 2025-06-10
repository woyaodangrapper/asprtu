using Asprtu.Memory.HybridContext.Contracts.Queue;
using Asprtu.Memory.HybridContext.Extensions;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Asprtu.Memory.HybridContext.Queue;

/// <summary>
/// 队列实现，支持 Dispose，极限/常规模式切换
/// </summary>
public abstract class QueueCache<T> : L1Cache
{
    protected ILogger<QueueCache<T>> Logger { get; }
    protected ISubject<T>? Subject { get; }
    protected SemaphoreSlim Signal { get; }

    protected ConcurrentQueue<QueueItem> Queue
    {
        get => _queue;
        private set => _queue = value;
    }

    private ConcurrentQueue<QueueItem> _queue;

    private readonly bool _mode;

    private bool _disposed;

    protected QueueCache(QueueOptions options, QueueContext<T> context, ILoggerFactory loggerFactory)
      : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(context);
        _mode = options.Mode;

        _queue = context.Queue;
        Signal = context.Signal;
        Subject = context.Subject;
        Logger = loggerFactory.CreateLogger<QueueCache<T>>();

        Events.Memory.Eviction += (sender, args) => TryCompact();
    }

    /// <summary>
    /// 入队（非阻塞）
    /// </summary>
    public void Enqueue(T item)
    {
        if (_mode)
        {
            // 极限模式：直接入队 item
            Queue.Enqueue(new(item!));
        }
        else
        {
            // 常规模式：存缓存，入队 key
            long key = SnowflakeId.NewSnowflakeId(); // 生成唯一 key
            _ = GetOrAdd(key, new QueueItem(item!), MemorySizeCalculator.TryOccupy(item), TimeSpan.FromSeconds(30)); // 存入缓存，30秒有效
            Queue.Enqueue(new(key)); // 队列里放 key
        }
    }

    /// <summary>
    /// 弹出单条（非阻塞）
    /// </summary>
    public bool Dequeue(out T? result, CancellationToken token)
    {
        result = default;
        if (!Dequeue(out QueueItem item, token))
        {
            return false;
        }

        if (item.Data is T data)
        {
            result = data;
            return true;
        }

        Type actualType = item.Data?.GetType() ?? typeof(object);
        throw new InvalidCastException($"无法将 {actualType} 转换为 {typeof(T)}");
    }

    public bool Dequeue(out QueueItem result, CancellationToken token)
    {
        result = default!;  // 默认值

        // 检查取消请求
        if (token.IsCancellationRequested)
        {
            // 取消请求时直接返回 false
            return false;
        }

        if (Queue.TryDequeue(out QueueItem? raw))
        {
            result = _mode
                ? raw : GetAndDel<QueueItem>(raw)
                    ?? throw new InvalidOperationException($"缓存未命中或已过期，Key={raw}");

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 标记为已丢弃
    /// </summary>
    /// <param name="key"></param>
    public void Discard(object key)
    {
        if (Get<T>(key) is { } item && item is IDiscardable discardable)
        {
            discardable.Discard = true;
        }
    }

    /// <summary>
    /// 批量标记为已丢弃
    /// </summary>
    /// <param name="keys"></param>
    public void Discard([NotNull] IEnumerable<object> keys)
    {
        foreach (object key in keys)
        {
            Discard(key);
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // 释放托管资源
                Signal.Dispose();
                if (Subject is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            // 释放非托管资源（如果有）

            _disposed = true;
            base.Dispose(disposing);
        }
    }

    private long lastCompactTicks;
    private int compactRunning;
    private const int MaxQueueSize = 100_000;

    /// <summary>
    /// Attempts to compact resources if sufficient time has elapsed since the last compaction.
    /// </summary>
    /// <remarks>This method ensures that compaction is performed at most once every 30 seconds and prevents
    /// concurrent compaction operations. It uses thread-safe mechanisms to manage timing and execution  state, ensuring
    /// that only one thread can perform compaction at a time.</remarks>
    private void TryCompact()
    {
        long now = DateTime.UtcNow.Ticks;
        if (now - Interlocked.Read(ref lastCompactTicks) < TimeSpan.FromSeconds(30).Ticks)
        {
            return;
        }

        if (Interlocked.Exchange(ref compactRunning, 1) == 1)
        {
            return;
        }

        try
        {
            _ = Interlocked.Exchange(ref lastCompactTicks, now);
            Compact();
        }
        finally
        {
            _ = Interlocked.Exchange(ref compactRunning, 0);
        }
    }

    /// <summary>
    /// Removes discarded items from the queue, creating a compacted version of the queue.
    /// </summary>
    /// <remarks>This method filters out items marked as discarded and replaces the current queue with a new
    /// queue containing only the remaining items. It ensures thread safety during the replacement operation.</remarks>
    private void Compact()
    {
        // Create a snapshot of the current queue
        QueueItem[] snapshot = [.. _queue];
        ConcurrentQueue<QueueItem> newQueue = new();

        // Filter out discarded items and enqueue the remaining ones
        foreach (QueueItem qi in snapshot)
        {
            if (!_mode)
            {
                QueueItem? mqi = Get<QueueItem>(qi.Data); // 走内存缓存获取原始数据
                if (mqi != null && !mqi.Discard)
                {
                    newQueue.Enqueue(qi);
                    Remove(qi.Data);
                }
            }
            else
            {
                // 极限模式：仅保留最近的 MaxQueueSize 条
                int count = snapshot.Length;
                int start = count > MaxQueueSize ? count - MaxQueueSize : 0;

                for (int i = start; i < count; i++)
                {
                    newQueue.Enqueue(snapshot[i]);
                }
            }
        }

        // Replace the old queue with the new one using Interlocked.Exchange
        _ = Interlocked.Exchange(ref Unsafe.As<ConcurrentQueue<QueueItem>, object>(ref _queue), newQueue);
    }
}