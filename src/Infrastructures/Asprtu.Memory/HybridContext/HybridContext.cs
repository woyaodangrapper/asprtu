using Asprtu.Memory.HybridContext.Contracts;
using Asprtu.Memory.HybridContext.Contracts.Queue;
using Asprtu.Memory.HybridContext.Queue;
using System.Diagnostics.CodeAnalysis;

namespace Asprtu.Memory.HybridContext;

public abstract class HybridContext<T> : QueueCache<ChangeEntry<T>>
{
    private readonly ILogger<HybridContext<T>> _logger;

    protected HybridContext(QueueOptions options, QueueContext<ChangeEntry<T>> context, ILoggerFactory loggerFactory)
        : base(options, context, loggerFactory)
        => _logger = loggerFactory.CreateLogger<HybridContext<T>>();

    protected HybridContext([NotNull] ILoggerFactory loggerFactory)
        : base(new QueueOptions(nameof(T)), new QueueContext<ChangeEntry<T>>(), loggerFactory)
        => _logger = loggerFactory.CreateLogger<HybridContext<T>>();

    /// <summary>
    /// 跟踪新实体为 Added
    /// </summary>
    public void Add(T entity)
    {
        ChangeEntry<T> entry = new(entity, ChangeType.Added);
        Enqueue(entry);
        OnEnqueued(entry);
    }

    /// <summary>
    /// 跟踪实体为 Updated
    /// </summary>
    public void Update(T entity)
    {
        ChangeEntry<T> entry = new(entity, ChangeType.Updated);
        Enqueue(entry);
        OnEnqueued(entry);
    }

    /// <summary>
    /// 跟踪实体为 Removed
    /// </summary>
    public void Remove(T entity)
    {
        ChangeEntry<T> entry = new(entity, ChangeType.Removed);
        Enqueue(entry);
        OnEnqueued(entry);
    }

    /// <summary>
    /// 批量提交所有跟踪的变更
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int dispatchedCount = 0;
        //Dequeue(out QueueItem result, cancellationToken);
        while (Dequeue(out QueueItem? result, cancellationToken))
        {
            ChangeEntry<T>? entry = result.Data as ChangeEntry<T>;

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (result == null || entry == null)
            {
                continue;
            }

            if (result is IDiscardable)
            {
                continue;
            }

            try
            {
                await DispatchAsync(entry.Entity, entry.Type).ConfigureAwait(false);
                dispatchedCount++;
            }
            catch (InvalidOperationException ex)
            {
                HybridContextLogging.LogDispatchInvalidOperation(_logger, entry.Type, entry.Entity!, ex);
            }
            catch (ArgumentException ex)
            {
                HybridContextLogging.LogDispatchArgumentError(_logger, entry.Type, entry.Entity!, ex);
            }
            catch (Exception ex)
            {
                HybridContextLogging.LogDispatchUnexpectedError(_logger, entry.Type, entry.Entity!, ex);
                throw; // Re-throw unexpected exceptions
            }
        }

        HybridContextLogging.LogSaveChangesApplied(_logger, dispatchedCount, typeof(T).Name);
        return dispatchedCount;
    }

    /// <summary>
    /// 入队后触发，可重写实现自动保存策略（如阈值、定时器等）
    /// </summary>
    public virtual void OnEnqueued(ChangeEntry<T> entry)
    { }

    /// <summary>
    /// 子类需要实现的调度逻辑，根据变更类型分发处理
    /// </summary>
    public abstract Task DispatchAsync(T entity, ChangeType changeType);
}

internal static class HybridContextLogging
{
    private static readonly Action<ILogger, ChangeType, object, Exception?> _logDispatchInvalidOperation =
        LoggerMessage.Define<ChangeType, object>(
            LogLevel.Error,
            new EventId(1, nameof(LogDispatchInvalidOperation)),
            "Dispatch failed due to invalid operation for {Type} on entity {Entity}");

    private static readonly Action<ILogger, ChangeType, object, Exception?> _logDispatchArgumentError =
        LoggerMessage.Define<ChangeType, object>(
            LogLevel.Error,
            new EventId(2, nameof(LogDispatchArgumentError)),
            "Dispatch failed due to argument error for {Type} on entity {Entity}");

    private static readonly Action<ILogger, ChangeType, object, Exception?> _logDispatchUnexpectedError =
        LoggerMessage.Define<ChangeType, object>(
            LogLevel.Error,
            new EventId(3, nameof(LogDispatchUnexpectedError)),
            "An unexpected error occurred during dispatch for {Type} on entity {Entity}");

    private static readonly Action<ILogger, int, string, Exception?> _logSaveChangesApplied =
        LoggerMessage.Define<int, string>(
            LogLevel.Information,
            new EventId(4, nameof(LogSaveChangesApplied)),
            "SaveChanges applied {Count} changes for {Type}");

    public static void LogDispatchInvalidOperation(ILogger logger, ChangeType type, object entity, Exception ex) => _logDispatchInvalidOperation(logger, type, entity, ex);

    public static void LogDispatchArgumentError(ILogger logger, ChangeType type, object entity, Exception ex) => _logDispatchArgumentError(logger, type, entity, ex);

    public static void LogDispatchUnexpectedError(ILogger logger, ChangeType type, object entity, Exception ex) => _logDispatchUnexpectedError(logger, type, entity, ex);

    public static void LogSaveChangesApplied(ILogger logger, int count, string typeName, Exception? ex = null) => _logSaveChangesApplied(logger, count, typeName, ex);
}