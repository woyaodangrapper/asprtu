using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Asprtu.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities.EventHub.Mqtt;

[NotLoaded]
public class MqttSub : MqttAbstractSubscriber, IMqttSub
{
    public MqttSub(IMqttConnection mqttConnection, ILogger<MqttSub> logger) : base(mqttConnection, logger)
    {
    }

    protected override string[] GetSubscribedTopics() => throw new NotImplementedException();

    protected override Task ProcessAsync(string topic, string message) => throw new NotImplementedException();
}