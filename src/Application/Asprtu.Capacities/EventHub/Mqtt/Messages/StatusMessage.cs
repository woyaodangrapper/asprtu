namespace Asprtu.Capacities.EventHub.Mqtt.Messages;

public class HellMessage : MqttAbstractMessage
{
    public string Say { get; set; } = "hello world!";
}