namespace Asprtu.Memory.HybridContext.Contracts.Queue;

public interface IQueueFactory<T>
{
    /// <summary> Creates a queue message publisher. </summary>
    IPublisher<T> CreatePublisher();

    /// <summary> Creates a queue message subscriber.</summary>
    ISubscriber<T> CreateSubscriber();
}