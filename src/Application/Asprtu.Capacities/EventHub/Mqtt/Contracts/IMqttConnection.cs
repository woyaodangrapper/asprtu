using MQTTnet;

namespace Asprtu.Capacities.EventHub.Mqtt.Contracts;

public interface IMqttConnection : IDisposable
{
    IMqttClient Connection { get; }
}