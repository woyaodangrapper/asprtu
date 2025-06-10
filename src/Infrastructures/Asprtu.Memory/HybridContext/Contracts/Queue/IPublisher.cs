namespace Asprtu.Memory.HybridContext.Contracts.Queue;

/// <summary>
/// Message publisher that publishes messages to the subscribers.
/// </summary>
public interface IPublisher<T> : IDisposable
{
    /// <summary>Enqueues the message to be published to the subscribers.</summary>
    bool TryEnqueue(T message);
}