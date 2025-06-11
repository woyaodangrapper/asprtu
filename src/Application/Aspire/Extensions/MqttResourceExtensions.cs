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
            string image = module.Config.Image ?? "emqx/nanomq";
            (string name, string tag) = image.Contains(':', StringComparison.Ordinal)
                     ? (image.Split(':')[0], image.Split(':')[1])
                     : (image, "latest");
            options = new MqttOptions
            {
                ContainerName = module.Name,
                Image = name,
                Tag = tag,
                Dashboard = module.Config.Dashboard,
                TcpPort = module.Config.Host.Port
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
        //Aspire.Hosting.Dcp.DefaultAspireNetworkName

        if (options.Dashboard)
        {
            _ = builder.AddGrafana().WaitFor(container);
        }

        //container = container.WithAnnotation(new ReplicaAnnotation(3));

        //if (options.UseCluster > 1)
        //{
        //    container = container.WithAnnotation(new ReplicaAnnotation(options.UseCluster));
        //}

        {
            container = container
                .WithEnvironment("NANO_WORKER_POOL_SIZE", "4")  //# Number of worker threads
                .WithEnvironment("NANO_MAX_CLIENTS", "100000") //# Max simultaneous clients
                .WithEnvironment("NANO_KEEPALIVE_THRESHOLD", "60")//# Keepalive check interval (seconds)
                .WithEnvironment("NANO_HB_INTERVAL", "30"); //# Healthcheck publish interval

            container = container
                   .WithEnvironment("NANOMQ_HTTP_SERVER_ENABLE", "true")
                   .WithEnvironment("NANOMQ_HTTP_SERVER_USERNAME", options.Username)
                   .WithEnvironment("NANOMQ_HTTP_SERVER_PASSWORD", options.Password);
        }
        //isProxied: false,

        container = container.WithHttpEndpoint(
            name: "http",
            port: options.HttpPort,
            targetPort: 8081);

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
            ? new DefaultResource(module.Config.Host.ToString(), module.Type, module.Name)
            : throw new InvalidOperationException("MqttClientModule is not configured.");
    }

    public static IResourceBuilder<ContainerResource> UseSystem(
        this IResourceBuilder<ContainerResource> builder)
    {
        return builder
            // 最大文件句柄数
            .WithSysctl("num_taskq_thread", "4")
            .WithSysctl("fs.max_taskq_thread", "8")
            .WithSysctl("fs.parallel", "2");
    }

    private static IResourceBuilder<ContainerResource> WithSysctl(
        this IResourceBuilder<ContainerResource> builder,
        string key,
        string value) =>
        // 添加 sysctl 参数到容器运行时参数
        builder.WithContainerRuntimeArgs($"--system.{key}", $"{value}");
}