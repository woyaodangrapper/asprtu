using Asprtu.Memory.Interprocess.Contracts;
using Asprtu.Memory.Interprocess.Extensions;
using static Cloudtoid.Contract;

namespace Asprtu.Memory.Interprocess.Queue;

/// <inheritdoc/>
public sealed class QueueFactory : IQueueFactory
{
    private readonly ILoggerFactory loggerFactory;

    /// <inheritdoc/>
    public QueueFactory()
        : this(NullLoggerFactory.Instance)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="QueueFactory"/> class.</summary>
    public QueueFactory(ILoggerFactory loggerFactory)
    {
        Util.Ensure64Bit();
        this.loggerFactory = CheckValue(loggerFactory, nameof(loggerFactory));
    }

    /// <inheritdoc/>
    public IPublisher CreatePublisher(QueueOptions options) =>
        new Publisher(CheckValue(options, nameof(options)), loggerFactory);

    /// <inheritdoc/>
    public ISubscriber CreateSubscriber(QueueOptions options) =>
        new Subscriber(CheckValue(options, nameof(options)), loggerFactory);
}