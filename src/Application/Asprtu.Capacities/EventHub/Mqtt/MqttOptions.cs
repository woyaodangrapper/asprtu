namespace Asprtu.Capacities.EventHub.Mqtt;

/// <summary>
/// 与 appsettings.json 中的 "Mqtt" 节对应
/// 支持用逗号分隔多个 Host:Port，以便做简单的故障切换
/// </summary>
public class MqttOptions
{
    /// <summary>
    /// 形如 "broker1.example.com:1883,broker2.example.com:1883" 的逗号分隔列表
    /// 如果只是单节点，也可以只写 "broker.example.com:1883"
    /// </summary>
    public string HostList { get; set; } = string.Empty;

    /// <summary>
    /// MQTT 客户端 ID，如果不填则会自动生成
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// 用户名（可选）
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 密码（可选）
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 是否开启 CleanSession（默认 true）
    /// </summary>
    public bool CleanSession { get; set; } = true;

    /// <summary>
    /// 连接超时时间（单位：秒），默认为 10 秒
    /// </summary>
    public int KeepAlivePeriodSeconds { get; set; } = 10;

    /// <summary>
    /// 是否启用 TLS/SSL（默认为 false）
    /// </summary>
    public bool UseTls { get; set; }
}