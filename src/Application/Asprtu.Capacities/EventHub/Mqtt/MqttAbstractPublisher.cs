using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Asprtu.Capacities.EventHub.Mqtt;

public abstract class MqttAbstractPublisher(IMqttConnection connection, ILoggerFactory loggerFactory)
{
    private readonly IMqttConnection _connection = connection;
    private readonly ILogger _logger = loggerFactory.CreateLogger<MqttAbstractPublisher>();

    public IMqttClient Client { get; } = connection.Connection;

    /// <summary>
    /// 发布原始字符串消息
    /// </summary>
    protected async Task PublishAsync(string topic, string payload, bool retain = false, int qos = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithRetainFlag(retain)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                .Build();

            IMqttClient client = _connection.Connection;
            if (!client.IsConnected)
            {
                throw new InvalidOperationException("MQTT 客户端未连接。无法发布消息。");
            }

            _ = await client.PublishAsync(message, cancellationToken).ConfigureAwait(false);
            LogPublishedRawMessage(_logger, topic, payload, null);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
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
    /// 发布默认消息（由子类实现具体逻辑）
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
            new EventId(100, nameof(MqttAbstractPublisher)),
            "Published raw message to topic '{Topic}': {Payload}");

    private static readonly Action<ILogger, string, Exception?> LogPublishRawError =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(102, nameof(MqttAbstractPublisher)),
            "Failed to publish raw message to topic '{Topic}'");

    #endregion LoggerMessages
}