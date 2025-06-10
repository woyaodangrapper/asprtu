using Asprtu.Memory.HybridContext.Contracts.Queue;
using System.Collections.Concurrent;

namespace Asprtu.Memory.HybridContext.Queue;

public class QueueFactory<T> : IQueueFactory<T>
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly QueueOptions _queueOptions;
    private readonly QueueContext<T> _queueContext;

    private static readonly ConcurrentDictionary<string, (QueueOptions Queue, QueueContext<T> Context)> _instance = new();

    public QueueFactory(string? name = null)
        : this(NullLoggerFactory.Instance, GetOrCreate(name ?? typeof(T).Name))
    {
    }

    public QueueFactory(ILoggerFactory loggerFactory, (QueueOptions Queue, QueueContext<T> Context) options)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _queueOptions = options.Queue ?? throw new ArgumentNullException(nameof(options));
        _queueContext = options.Context ?? throw new ArgumentNullException(nameof(options));
    }

    private static (QueueOptions, QueueContext<T>) GetOrCreate(string name)
        => _instance.GetOrAdd(name, _ => (new QueueOptions(name), new QueueContext<T>()));

    /// <inheritdoc/>
    public IPublisher<T> CreatePublisher(QueueOptions options, QueueContext<T> context) =>
        new Publisher<T>(options, context, _loggerFactory);

    /// <inheritdoc/>
    public ISubscriber<T> CreateSubscriber(QueueOptions options, QueueContext<T> context) =>
        new Subscriber<T>(options, context, _loggerFactory);

    /// <inheritdoc/>
    public IPublisher<T> CreatePublisher() =>
        new Publisher<T>(_queueOptions, _queueContext, _loggerFactory);

    /// <inheritdoc/>
    public ISubscriber<T> CreateSubscriber() =>
        new Subscriber<T>(_queueOptions, _queueContext, _loggerFactory);
}