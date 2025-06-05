using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Asprtu.Capacities.EventHub.Mqtt.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Protocol;
using System.Text;

namespace Asprtu.Capacities.EventHub.Mqtt;

public abstract class MqttAbstractSubscriber(IMqttConnection mqttConnection, ILogger<MqttAbstractSubscriber> logger) : BackgroundService
{
    private readonly IMqttConnection _mqttConnection = mqttConnection;
    private readonly ILogger<MqttAbstractSubscriber> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IMqttClient client = _mqttConnection.Connection;
        string[] topics = GetSubscribedTopics();

        foreach (string topic in topics)
        {
            _ = await client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic, MqttQualityOfServiceLevel.AtLeastOnce)
                .Build(), stoppingToken).ConfigureAwait(false);

            Util.LogInformationMessage(_logger, $"Subscribed to topic: {topic}", null);
        }

        client.ApplicationMessageReceivedAsync += async e =>
        {
            string? topic = e.ApplicationMessage?.Topic;

            string payload = e.ApplicationMessage?.Payload is { Length: > 0 } bytes
                ? Encoding.UTF8.GetString(bytes)
                : string.Empty;

            Util.LogInformationMessage(_logger, $"Received MQTT message. Topic: {topic}, Payload: {payload}", null);

            try
            {
                if (!string.IsNullOrWhiteSpace(topic))
                {
                    await ProcessAsync(topic, payload).ConfigureAwait(false);
                }
            }
            catch (ArgumentException ex)
            {
                Util.LogErrorMessage(_logger, $"Argument error processing message from topic: {topic}", ex);
            }
            catch (InvalidOperationException ex)
            {
                Util.LogErrorMessage(_logger, $"Invalid operation processing message from topic: {topic}", ex);
            }
            catch (Exception ex)
            {
                Util.LogErrorMessage(_logger, $"Unexpected error processing message from topic: {topic}", ex);
                throw; // Re-throw the exception to ensure it is not swallowed
            }
        };
    }

    /// <summary>
    /// 返回要订阅的主题列表
    /// </summary>
    protected abstract string[] GetSubscribedTopics();

    /// <summary>
    /// 消息处理逻辑
    /// </summary>
    protected abstract Task ProcessAsync(string topic, string message);

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _mqttConnection.Dispose();
            }
            _disposed = true;
        }
    }

    public override void Dispose()
    {
        Dispose(disposing: true);
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}