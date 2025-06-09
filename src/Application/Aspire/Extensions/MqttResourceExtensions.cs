using Aspire.Contracts;
using Aspire.Onboarding;
using Asprtu.Core.Extensions;
using Asprtu.Core.Extensions.Module;

namespace Aspire.Extensions;

internal static class MqttResourceExtensions
{
    /// <summary>
    /// 将 EMQX 容器注册到当前 DistributedApplication 中,优先使用配置文件中的 mqtt-server 模块（如存在）
    /// </summary>
    /// <param name="builder">Aspire 分布式应用构建器</param>
    /// <param name="configure">可选的 MqttOptions 配置回调</param>
    /// <returns>返回构建后的 IResourceBuilder&lt;ContainerResource&gt;</returns>
    public static IResourceBuilder<ContainerResource> AddMqtt(
        this IDistributedApplicationBuilder builder,
        Action<MqttOptions>? configure = null)
    {
        //builder.Services.TryAddLifecycleHook<ConnectToMqttNetHook>();
        ModuleProvider moduleProvider = ModuleLoaderExtensions.TryLoad(builder.Configuration);
        MqttOptions options;
        if (moduleProvider.TryGet(out IModule<MqttServerConfig>? mqttServerModule) && mqttServerModule is { } module)
        {
            string image = module.Config.Image ?? "emqx/emqx";
            (string name, string tag) = image.Contains(':', StringComparison.Ordinal)
                     ? (image.Split(':')[0], image.Split(':')[1])
                     : (image, "latest");

            options = new MqttOptions
            {
                ContainerName = module.Name,
                Image = name,
                Tag = tag,
                TcpPort = module.Config.BrokerUrl.Port,
                UseTuning = false // 默认不启用调优
            };
        }
        else
        {
            options = new MqttOptions();
        }

        configure?.Invoke(options);

        IResourceBuilder<ContainerResource> container
            = builder.AddContainer(options.ContainerName, options.Image, options.Tag);

        if (!string.IsNullOrWhiteSpace(options.NetworkName))
        {
            // aspire 暂不支持自定义的网络 by https://github.com/dotnet/aspire/issues/850

            //string alias = string.IsNullOrWhiteSpace(options.NetworkAlias)   // 如果 NetworkAlias 为空，则让 containerName 作为默认别名
            //    ? options.ContainerName
            //    : options.NetworkAlias;

            // 可能预期的实现是这样的：
            //var net1 = builder.AddNetwork("net1");
            //var net2 = builder.AddNetwork("net2");

            //var app1 = builder.AddContainer("app1", "image1")
            //                  .AttachTo(net1);

            //var app2 = builder.AddContainer("app2", "image2")
            //                  .AttachTo(net1)
            //                  .AttachTo(net2);

            //var app2 = builder.AddContainer("app3", "image3")
            //                  .AttachTo(net2);
        }
        if (options.UseTuning)
        {
            container = container.UseTuning();
        }
        //container = container.WithAnnotation(new ReplicaAnnotation(3));

        //if (options.UseCluster > 1)
        //{
        //    container = container.WithAnnotation(new ReplicaAnnotation(options.UseCluster));
        //}
        // 4. 如果启用常见插件，设置环境变量让 EMQX 加载 Dashboard & Management 插件
        if (options.EnablePlugins)
        {
            container = container
                .WithEnvironment("EMQX_DASHBOARD__DEFAULT_USERNAME", options.DashboardUser)
                .WithEnvironment("EMQX_DASHBOARD__DEFAULT_PASSWORD", options.DashboardPassword)
                // EMQX 从 v5 起可以直接用 EMQX_LOADED_PLUGINS 来加载插件
                .WithEnvironment("EMQX_LOADED_PLUGINS", "emqx_dashboard,emqx_management");
        }
        //Listener tcp:default on 0.0.0.0:1883 started.
        //Listener ssl:default on 0.0.0.0:8883 started.
        //Listener ws:default on 0.0.0.0:8083 started.
        //Listener wss:default on 0.0.0.0:8084 started.
        container = container.WithHttpEndpoint(
            name: "mqtt-dashboard",
            port: options.DashboardPort,
            targetPort: 18083);

        container = container.WithEndpoint(
            name: "mqtt-tcp",
            scheme: "tcp",
            port: options.TcpPort,
            targetPort: 1883);

        container = container.WithEndpoint(
            name: "mqtts-tcp",
            scheme: "tcp",
            port: options.TcpPort + 1,
            targetPort: 8883);

        container = container.WithHttpEndpoint(
            name: "mqtt-ws",
            port: options.WebSocketPort,
            targetPort: 8083);

        container = container.WithHttpEndpoint(
            name: "mqtt-wss",
            port: options.WebSocketPort + 1,
            targetPort: 8084);
        return container;
    }

    internal static DefaultResource CreateMqttResource(this IDistributedApplicationBuilder builder)
    {
        ModuleProvider moduleProvider = ModuleLoaderExtensions.TryLoad(builder.Configuration);
        return moduleProvider.TryGet(out IModule<MqttServerConfig>? mqttServerModule) && mqttServerModule is { } module
            ? new DefaultResource(module.Config.BrokerUrl.ToString(), module.Type, module.Name)
            : throw new InvalidOperationException("MqttClientModule is not configured.");
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