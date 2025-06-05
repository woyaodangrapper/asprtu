namespace Aspire.Contracts;

/// <summary>
/// EMQX 容器默认设置类，可通过 AddMqtt 时传入以覆盖默认值
/// </summary>
internal class MqttOptions
{
    /// <summary>
    /// 容器资源名称，默认为 "emqx"
    /// </summary>
    public string ContainerName { get; set; } = "asprtu-emqx";

    /// <summary>
    /// 启用 emqx 的 linux 内核调优
    /// </summary>
    public bool UseTuning { get; set; } = false;

    /// <summary>
    /// 是否启用 EMQX 的集群模式
    /// </summary>
    public int UseCluster { get; set; } = 1;

    /// <summary>
    /// Docker 镜像名称，默认为 "emqx"
    /// （等同于 hub.docker.com 上的 emqx/emqx）
    /// </summary>
    public string Image { get; set; } = "emqx";

    /// <summary>
    /// 镜像标签，默认为 "latest"
    /// </summary>
    public string Tag { get; set; } = "latest";

    /// <summary>
    /// 是否指定自定义 Docker 网络名称。
    /// 默认为 null，使用 Aspire 默认网络。
    /// </summary>
    public string? NetworkName { get; set; } = null;

    /// <summary>
    /// 容器在自定义网络中的别名，用于集群互联时给 Erlang 节点起 Hostname 使用。
    /// 默认为 null（此时会将 ContainerName 作为内部别名）。
    /// </summary>
    public string? NetworkAlias { get; set; } = null;

    /// <summary>
    /// Dashboard 用户名，默认为 "admin"
    /// </summary>
    public string DashboardUser { get; set; } = "admin";

    /// <summary>
    /// Dashboard 密码，默认为 "public"
    /// </summary>
    public string DashboardPassword { get; set; } = "public";

    /// <summary>
    /// 是否加载 Dashboard、Management 等插件，默认为 true
    /// </summary>
    public bool EnablePlugins { get; set; } = true;

    /// <summary>
    /// MQTT TCP 端口映射：宿主机端口，默认为 1883
    /// </summary>
    public int TcpPort { get; set; } = 1883;

    /// <summary>
    /// MQTT WebSocket 端口映射：宿主机端口，默认为 8083（容器内部 8083）
    /// </summary>
    public int WebSocketPort { get; set; } = 8083;

    /// <summary>
    /// Dashboard 面板端口映射：宿主机端口，默认为 18083（容器内部 18083）
    /// </summary>
    public int DashboardPort { get; set; } = 18083;
}