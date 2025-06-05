namespace Asprtu.Capacities.EventHub.Mqtt.Messages;

public class StatusMessage : MqttAbstractMessage
{
    public string Status { get; set; } = "default";
}