using Aspire.Contracts;

namespace Aspire.Extensions;

internal static class MqttResourceExtensions
{
    /// <summary>
    /// 将 EMQX 容器注册到当前 DistributedApplication 中，并开放以下默认端口：
    ///  - MQTT TCP: 1883
    ///  - MQTT WebSocket: 8083
    ///  - Dashboard 面板: 18083
    ///
    /// 可以通过可选回调参数 overrideOptions 修改默认配置。
    /// </summary>
    /// <param name="builder">Aspire 分布式应用构建器</param>
    /// <param name="configure">可选的 MqttOptions 配置回调</param>
    /// <returns>返回构建后的 IResourceBuilder&lt;ContainerResource&gt;</returns>
    public static IResourceBuilder<ContainerResource> AddMqtt(
        this IDistributedApplicationBuilder builder,
        Action<MqttOptions>? configure = null)
    {
        // 1. 准备默认配置
        MqttOptions options = new();
        configure?.Invoke(options);

        // 2. 调用 AddContainer 注册容器资源
        IResourceBuilder<ContainerResource> container = builder.AddContainer(options.ContainerName, options.Image, options.Tag);

        // 3. 如果指定了自定义 Docker 网络，则挂载到该网络，并加上别名
        if (!string.IsNullOrWhiteSpace(options.NetworkName))
        {
            // 如果 NetworkAlias 为空，则让 containerName 作为默认别名
            string alias = string.IsNullOrWhiteSpace(options.NetworkAlias)
                ? options.ContainerName
                : options.NetworkAlias;

            container = container
                .WithContainerRuntimeArgs("--network", options.NetworkName, "--network-alias", alias)
                //.WithNetwork(options.NetworkName)
                //.WithNetworkAlias(alias)
                ;
        }

        if (options.UseTuning)
        {
            container = container.UseTuning();
        }
        // 4. 如果启用常见插件，设置环境变量让 EMQX 加载 Dashboard & Management 插件
        if (options.EnablePlugins)
        {
            container = container
                .WithEnvironment("EMQX_DASHBOARD__DEFAULT_USERNAME", options.DashboardUser)
                .WithEnvironment("EMQX_DASHBOARD__DEFAULT_PASSWORD", options.DashboardPassword)
                // EMQX 从 v5 起可以直接用 EMQX_LOADED_PLUGINS 来加载插件
                .WithEnvironment("EMQX_LOADED_PLUGINS", "emqx_dashboard,emqx_management");
        }

        // 5. 暴露并映射 Dashboard HTTP 端口
        container = container.WithHttpEndpoint(
            name: "mqtt-dashboard",
            port: options.DashboardPort,
            targetPort: 18083); // EMQX 容器内部 Dashboard 默认绑定 18083

        // 6. 暴露并映射 MQTT TCP 端口
        container = container.WithEndpoint(
            name: "mqtt-tcp",
            scheme: "tcp",
            port: options.TcpPort,
            targetPort: 1883);

        // 7. 暴露并映射 MQTT WebSocket 端口
        container = container.WithHttpEndpoint(
            name: "mqtt-websocket",
            port: options.WebSocketPort,
            targetPort: 8083);

        return container;
    }

    /// <summary>
    /// 为 EMQX 容器启用一系列 Linux 内核参数（sysctl 调优），
    /// 参考：https://docs.emqx.com/en/emqx/latest/performance/tune.html
    /// </summary>
    /// <param name="builder">IResourceBuilder&lt;ContainerResource&gt;（EMQX 容器资源构建器）</param>
    /// <returns>返回更新后的 IResourceBuilder&lt;ContainerResource&gt;</returns>
    public static IResourceBuilder<ContainerResource> UseTuning(
        this IResourceBuilder<ContainerResource> builder)
    {
        return builder
            // 最大文件句柄数
            .WithSysctl("fs.file-max", "2097152")
            .WithSysctl("fs.nr_open", "2097152")
            // 网络队列深度
            .WithSysctl("net.core.somaxconn", "32768")
            .WithSysctl("net.ipv4.tcp_max_syn_backlog", "16384")
            .WithSysctl("net.core.netdev_max_backlog", "16384")
            // 本地端口范围
            .WithSysctl("net.ipv4.ip_local_port_range", "1000 65535")
            // 默认/最大缓冲区
            .WithSysctl("net.core.rmem_default", "262144")
            .WithSysctl("net.core.wmem_default", "262144")
            .WithSysctl("net.core.rmem_max", "16777216")
            .WithSysctl("net.core.wmem_max", "16777216")
            .WithSysctl("net.core.optmem_max", "16777216")
            // TCP 缓冲区
            .WithSysctl("net.ipv4.tcp_rmem", "1024 4096 16777216")
            .WithSysctl("net.ipv4.tcp_wmem", "1024 4096 16777216")
            // TIME-WAIT 桶限制
            .WithSysctl("net.ipv4.tcp_max_tw_buckets", "1048576")
            // TCP 连接等待时间
            .WithSysctl("net.ipv4.tcp_fin_timeout", "15");
    }

    private static IResourceBuilder<ContainerResource> WithSysctl(
        this IResourceBuilder<ContainerResource> builder,
        string key,
        string value) =>
        // 添加 sysctl 参数到容器运行时参数
        builder.WithContainerRuntimeArgs("--sysctl", $"{key}={value}");
}