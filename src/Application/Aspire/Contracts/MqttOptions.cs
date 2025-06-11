namespace Aspire.Contracts;

/// <summary>
/// 容器默认设置类，可通过 AddMqtt 时传入以覆盖默认值
/// </summary>
internal class MqttOptions
{
    /// <summary>
    /// 容器资源名称，默认为 "asprtu-mqtt"
    /// </summary>
    public string ContainerName { get; set; } = string.Empty;

    /// <summary>
    /// Docker 镜像名称
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// 镜像标签，默认为 "latest"
    /// </summary>
    public string Tag { get; set; } = "latest";

    /// <summary>
    /// 启动面板开关
    /// </summary>
    public bool Dashboard { get; set; }

    /// <summary>
    /// 是否指定自定义 Docker 网络名称。
    /// 默认为 null，使用 Aspire 默认网络。
    /// </summary>
    public string? NetworkName { get; set; }

    /// <summary>
    /// 容器在自定义网络中的别名，用于集群互联时给 Erlang 节点起 Hostname 使用。
    /// 默认为 null（此时会将 ContainerName 作为内部别名）。
    /// </summary>
    public string? NetworkAlias { get; set; }

    /// <summary>
    /// 用户名，默认为 "admin"
    /// </summary>
    public string Username { get; set; } = "admin";

    /// <summary>
    /// 密码，默认为 "public"
    /// </summary>
    public string Password { get; set; } = "public";

    /// <summary>
    /// http 服务器的端口（默认值：8081）。
    /// </summary>
    public int HttpPort { get; set; } = 8081;

    /// <summary>
    /// MQTT TCP 端口映射：宿主机端口，默认为 1883
    /// </summary>
    public int TcpPort { get; set; } = 1883;

    /// <summary>
    /// MQTT WebSocket 端口映射：宿主机端口，默认为 8083（容器内部 8083）
    /// </summary>
    public int WebSocketPort { get; set; } = 8083;
}