using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace Asprtu.Memory.HybridContext.Contracts.Queue;

public class QueueContext<T>
{
    public ConcurrentQueue<QueueItem> Queue { get; } = new();
    public SemaphoreSlim Signal { get; } = new(0);
    public ISubject<T>? Subject { get; set; } = new Subject<T>();
}

public interface IDiscardable
{
    bool Discard { get; set; }
}

public class QueueItem(object data) : IDiscardable
{
    public object Data { get; } = data;
    private volatile bool _discard;

    public bool Discard
    {
        get => _discard;
        set => _discard = value;
    }
}