using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Asprtu.Memory.HybridContext;
using Asprtu.Memory.HybridContext.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Asprtu.Capacities.EventHub.Mqtt;

public class MqttContext<T> : HybridContext<T>, IMqttContext<T>
    where T : MqttAbstractMessage, new()
{
    private readonly ILogger<MqttContext<T>> _logger;
    private readonly Lazy<IMqttPub> _publisher;
    private readonly Subject<ChangeEntry<T>> _subject;
    private bool _disposed;

    public MqttContext(
        IServiceProvider provider,
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _logger = loggerFactory.CreateLogger<MqttContext<T>>();
        _publisher = new Lazy<IMqttPub>(() => ActivatorUtilities.CreateInstance<IMqttPub>(provider));
        _subject = new Subject<ChangeEntry<T>>();

        _ = _subject
            .Buffer(TimeSpan.FromSeconds(5), 100)
            .Where(batch => batch.Count > 0)
            .Subscribe(async batch =>
            {
                foreach (ChangeEntry<T>? entry in batch)
                {
                    await DispatchAsync(entry.Entity, entry.Type).ConfigureAwait(false);
                }
                Discard(batch.Select(b => b.GetHashCode()));
            });
    }

    public override async Task DispatchAsync(T entity, ChangeType changeType)
        => await _publisher.Value.TryPublishAsync(entity).ConfigureAwait(false);

    public override void OnEnqueued(ChangeEntry<T> entry)
        => _subject.OnNext(entry);

    /// <summary>
    /// Dispose 方法，用于释放资源
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _subject?.Dispose();
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }
}