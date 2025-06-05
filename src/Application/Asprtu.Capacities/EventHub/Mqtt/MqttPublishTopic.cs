namespace Asprtu.Capacities.EventHub.Mqtt;

public class MqttPublishTopic
{
    /// <summary> 主题 </summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary> 服务质量级别（0=最多一次, 1=至少一次, 2=仅一次）</summary>
    public int Qos { get; set; } = 1;

    /// <summary> 是否保留（retain）消息 </summary>
    public bool Retain { get; set; }

    /// <summary> 是否启用此配置（用于动态控制）</summary>
    public bool Enabled { get; set; } = true;
}