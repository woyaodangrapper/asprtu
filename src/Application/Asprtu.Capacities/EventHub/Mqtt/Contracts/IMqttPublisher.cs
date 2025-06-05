namespace Asprtu.Capacities.EventHub.Mqtt.Contracts;

public interface IMqttPublisher
{
    Task PublishAsync(string topic, string payload, bool retain = false, int qos = 1);

    Task PublishJsonAsync<T>(string topic, T obj, bool retain = false, int qos = 1);
}