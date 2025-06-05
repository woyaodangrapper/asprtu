using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities.EventHub.Mqtt;

public abstract class MqttAbstractPublisher(IMqttPublisher publisher, ILogger logger)
{
    private readonly IMqttPublisher _publisher = publisher;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// 发布原始字符串消息
    /// </summary>
    protected async Task PublishAsync(string topic, string payload, bool retain = false, int qos = 1)
    {
        try
        {
            await _publisher.PublishAsync(topic, payload, retain, qos).ConfigureAwait(false);
            LogPublishedRawMessage(_logger, topic, payload, null);
        }
        catch (ArgumentException ex)
        {
            LogPublishRawError(_logger, topic, ex);
            throw;
        }
        catch (InvalidOperationException ex)
        {
            LogPublishRawError(_logger, topic, ex);
            throw;
        }
        catch (Exception ex)
        {
            LogPublishRawError(_logger, topic, ex);
            throw;
        }
    }

    /// <summary>
    /// 发布当前实现定义的默认消息
    /// </summary>
    public abstract Task PublishDefaultAsync();

    /// <summary>
    /// 获取默认的 Topic
    /// </summary>
    protected abstract string GetDefaultTopic();

    #region LoggerMessages

    private static readonly Action<ILogger, string, string, Exception?> LogPublishedRawMessage =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(100, nameof(AbstractMqttPublisher)),
            "Published raw message to topic '{Topic}': {Payload}");

    private static readonly Action<ILogger, string, Exception?> LogPublishRawError =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(102, nameof(AbstractMqttPublisher)),
            "Failed to publish raw message to topic '{Topic}'");

    #endregion LoggerMessages
}