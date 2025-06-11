using DotNetCore.CAP;

namespace Asprtu.Capacities.EventHub.Cap;

public class CapPublisher(ICapPublisher publisher) : IEventPublisher
{
    public async Task PublishAsync<T>(T contentObj, string? callbackName = null, CancellationToken cancellationToken = default) where T : class
        => await publisher.PublishAsync(typeof(T).Name, contentObj, callbackName, cancellationToken).ConfigureAwait(false);

    public async Task PublishAsync<T>(T contentObj, IDictionary<string, string?> headers, CancellationToken cancellationToken = default) where T : class
        => await publisher.PublishAsync(typeof(T).Name, contentObj, headers, cancellationToken).ConfigureAwait(false);

    public void Publish<T>(T contentObj, string? callbackName = null) where T : class
        => publisher.Publish(typeof(T).Name, contentObj, callbackName);

    public void Publish<T>(T contentObj, IDictionary<string, string?> headers) where T : class
        => publisher.Publish(typeof(T).Name, contentObj, headers);

    public async Task PublishDelayAsync<T>(TimeSpan delayTime, T? contentObj, IDictionary<string, string?> headers, CancellationToken cancellationToken = default)
        => await publisher.PublishDelayAsync(delayTime, typeof(T).Name, contentObj, headers, cancellationToken).ConfigureAwait(false);

    public async Task PublishDelayAsync<T>(TimeSpan delayTime, T? contentObj, string? callbackName = null, CancellationToken cancellationToken = default)
        => await publisher.PublishDelayAsync(delayTime, typeof(T).Name, contentObj, callbackName, cancellationToken).ConfigureAwait(false);

    public void PublishDelay<T>(TimeSpan delayTime, T? contentObj, IDictionary<string, string?> headers)
        => publisher.PublishDelay(delayTime, typeof(T).Name, contentObj, headers);

    public void PublishDelay<T>(TimeSpan delayTime, T? contentObj, string? callbackName = null)
        => publisher.PublishDelay(delayTime, typeof(T).Name, contentObj, callbackName);

    public async Task PublishAsync<T>(string name, T? contentObj, string? callbackName = null, CancellationToken cancellationToken = default)
        => await publisher.PublishAsync(name, contentObj, callbackName, cancellationToken).ConfigureAwait(false);

    public async Task PublishAsync<T>(string name, T? contentObj, IDictionary<string, string?> headers, CancellationToken cancellationToken = default)
        => await publisher.PublishAsync(name, contentObj, headers, cancellationToken).ConfigureAwait(false);

    public void Publish<T>(string name, T? contentObj, string? callbackName = null)
        => publisher.Publish(name, contentObj, callbackName);

    public void Publish<T>(string name, T? contentObj, IDictionary<string, string?> headers)
        => publisher.Publish(name, contentObj, headers);

    public async Task PublishDelayAsync<T>(TimeSpan delayTime, string name, T? contentObj, IDictionary<string, string?> headers, CancellationToken cancellationToken = default)
        => await publisher.PublishDelayAsync(delayTime, name, contentObj, headers, cancellationToken).ConfigureAwait(false);

    public async Task PublishDelayAsync<T>(TimeSpan delayTime, string name, T? contentObj, string? callbackName = null, CancellationToken cancellationToken = default)
        => await publisher.PublishDelayAsync(delayTime, name, contentObj, callbackName, cancellationToken).ConfigureAwait(false);

    public void PublishDelay<T>(TimeSpan delayTime, string name, T? contentObj, IDictionary<string, string?> headers)
        => publisher.PublishDelay(delayTime, name, contentObj, headers);

    public void PublishDelay<T>(TimeSpan delayTime, string name, T? contentObj, string? callbackName = null)
        => publisher.PublishDelay(delayTime, name, contentObj, callbackName);
}