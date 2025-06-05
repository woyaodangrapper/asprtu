using Asprtu.Capacities.EventHub.Mqtt.Configuration;
using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Asprtu.Capacities.EventHub.Mqtt.Helpers;
using Asprtu.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Qos = MQTTnet.Protocol.MqttQualityOfServiceLevel;

namespace Asprtu.Capacities.EventHub.Mqtt;

[Asprtus]
public class MqttPub : MqttAbstractPublisher, IMqttPub

{
    private readonly ILogger<MqttPub> _logger;

    public MqttPub(IMqttConnection connection, ILoggerFactory loggerFactory)
        : base(connection, loggerFactory)
    {
        _logger = (loggerFactory ?? NullLoggerFactory.Instance).CreateLogger<MqttPub>();
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection), "MQTT connection cannot be null.");
        }
    }

    public override Task PublishDefaultAsync()
    {
        string topic = GetDefaultTopic();
        string payload = "{\"status\":\"default\"}";
        return PublishAsync(topic, payload, retain: false, qos: 1);
    }

    protected override string GetDefaultTopic() => "default/topic";

    public async Task<bool> TryPublishAsync<T>([NotNull] T data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
    where T : MqttAbstractMessage, new()
    {
        byte[] payload;
        try
        {
            if (MqttJsonContext.Default.GetTypeInfo(typeof(T)) is not { } typeInfo)
            {
                throw new NotSupportedException($"类型 {typeof(T).Name} 未在 MqttJsonContext 中注册，无法 AOT 序列化。");
            }
            payload = JsonSerializer.SerializeToUtf8Bytes(data, typeInfo);
        }
        catch (NotSupportedException ex)
        {
            _logger.LogWarning(ex.Message);
            return false;
        }

        return await TryPublishAsync(payload, rule, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> TryPublishAsync(byte[] data, MqttPubRule? rule, CancellationToken cancellationToken)
    {
        string topic = rule?.Topic ?? GetDefaultTopic();

        Qos qosLevel = rule?.Qos != null
            ? (Qos)rule.Qos
            : Qos.AtMostOnce;

        MqttApplicationMessage message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(data)
            .WithRetainFlag(rule?.Retain ?? false)
            .WithQualityOfServiceLevel(qosLevel)
            .Build();

        MqttClientPublishResult result = await Client.PublishAsync(message, cancellationToken).ConfigureAwait(false);
        return result.ReasonCode == MqttClientPublishReasonCode.Success;
    }

    Task IMqttPub.PublishAsync(string topic, string payload, bool retain, int qos) => PublishAsync(topic, payload, retain, qos);

    public Task<bool> TryPublishAsync(int data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(BitConverter.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(float data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(double data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(bool data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(short data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(long data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(byte data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(char data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(decimal data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(string data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);

    public Task<bool> TryPublishAsync(DateTime data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        => TryPublishAsync(Util.GetBytes(data), rule, cancellationToken);
}