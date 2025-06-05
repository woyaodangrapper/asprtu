namespace Asprtu.Capacities.EventHub.Mqtt;

public class MqttSubscribeTopic
{
    /// <summary> 订阅的主题，可使用通配符，如 "sensors/+" </summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary> QoS 等级 </summary>
    public int Qos { get; set; } = 1;

    /// <summary> 是否自动重连后重新订阅 </summary>
    public bool AutoResubscribe { get; set; } = true;

    /// <summary> 是否启用此订阅 </summary>
    public bool Enabled { get; set; } = true;
}