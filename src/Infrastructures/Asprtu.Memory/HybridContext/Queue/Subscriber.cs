using Asprtu.Memory.HybridContext.Contracts.Queue;

namespace Asprtu.Memory.HybridContext.Queue;

public class Subscriber<T> : QueueCache<T>, ISubscriber<T>
{
    public Subscriber(QueueOptions options, QueueContext<T> context, ILoggerFactory loggerFactory)
      : base(options, context, loggerFactory)
    { }

    public IObservable<T>? Observable => Subject;

    public bool TryDequeue(out T? message)
    {
        message = default;
        Signal.Wait(0);
        try
        {
            return Dequeue(out message, default);
        }
        catch (OperationCanceledException) // Catch specific exception
        {
            return false;
        }
    }

    public bool TryDequeue(out T? message, CancellationToken cancellationToken)
    {
        message = default;

        Signal.Wait(cancellationToken);
        try
        {
            return Dequeue(out message, cancellationToken);
        }
        catch (OperationCanceledException) // Catch specific exception
        {
            return false;
        }
    }

    public async Task<(bool success, T? item)> TryDequeueAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Signal.WaitAsync(cancellationToken).ConfigureAwait(false);
            return Dequeue(out T? item, cancellationToken) ? ((bool success, T? item))(true, item) : default;
        }
        catch (OperationCanceledException)
        {
            return default;
        }
    }

    public async Task<IEnumerable<T>> TryDequeueBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        List<T> list = new(batchSize);
        for (int i = 0; i < batchSize; i++)
        {
            (bool success, T? item) = await TryDequeueAsync(cancellationToken).ConfigureAwait(false);
            if (success && item is not null)
            {
                list.Add(item);
            }
            else
            {
                break;
            }
        }
        return list;
    }

    public bool TryDequeueBatch(out IEnumerable<T> values, int batchSize, CancellationToken cancellationToken)
    {
        List<T> list = new List<T>();

        for (int i = 0; i < batchSize; i++)
        {
            if (TryDequeue(out T? item, cancellationToken) && item is not null)
            {
                list.Add(item);
            }
            else
            {
                // 极端竞态：信号到达但未取到元素，则跳出
                break;
            }
        }

        values = list;
        return list.Count > 0;
    }

    public IList<T> DequeueAll()
    {
        List<T> list = [];
        while (TryDequeue(out T? item) && item is not null)
        {
            list.Add(item);
            Signal.Wait();
        }
        return list;
    }
}