using Asprtu.Capacities.EventHub.Mqtt;
using Asprtu.Capacities.EventHub.Mqtt.Configuration;
using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Asprtu.Capacities.Registrar;
using Asprtu.Core.Extensions;
using Asprtu.Core.Extensions.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 添加 Asprtu 远程终端单元(智能硬件)控制功能。
    /// </summary>
    /// <typeparam name="TBuilder">表示 TBuilder 类型，必须实现 IHostApplicationBuilder 接口。</typeparam>
    /// <param name="builder">TBuilder 实例，用于添加 Asprtu 功能。</param>
    /// <returns>返回原始的 TBuilder 实例，支持链式调用。</returns>
    public static IHostApplicationBuilder AddAsprtu<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        return RegistrarFactory.Create(builder)
            .UseAsprtus();
    }

    /// <summary>
    /// 注册并初始化 MQTT 客户端连接服务，将 <see cref="IMqttConnection"/> 添加为单例到依赖注入容器中，
    /// 同时注册 MQTT 上下文 <see cref="MqttContext{T}"/> 用于支持实体消息的调度与缓存处理。
    /// </summary>
    /// <typeparam name="TBuilder">主机构建器类型，必须实现 <see cref="IHostApplicationBuilder"/>。</typeparam>
    /// <param name="builder">用于构建应用程序主机的构建器实例。</param>
    /// <param name="configure">可选的 MQTT 配置委托，用于自定义 <see cref="MqttOptions"/>。</param>
    /// <returns>返回用于链式调用的 <paramref name="builder"/> 实例。</returns>
    public static TBuilder AddMqtt<TBuilder>(
        this TBuilder builder,
        Action<MqttOptions>? configure = null) where TBuilder : IHostApplicationBuilder
    {
        // 配置选项
        MqttOptions options = new();
        configure?.Invoke(options);

        // 注册 MQTT 连接为单例
        _ = builder.Services.AddSingleton<IMqttConnection>(sp =>
        {
            ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
            ILogger<IMqttConnection> logger = loggerFactory.CreateLogger<IMqttConnection>();
            ModuleProvider moduleProvider = ModuleLoaderExtensions.TryLoad(configuration);

            if (string.IsNullOrEmpty(options.HostList) && moduleProvider.TryGet(out IModule<MqttClientConfig>? mqttClientConfig))
            {
                options.HostList = mqttClientConfig!.Config.BrokerUrl.ToString();
                _mqttServerAddressOverridden(logger, null);
            }

            return string.IsNullOrEmpty(options.HostList)
                ? throw new ArgumentNullException(nameof(configure), "MQTT 服务器地址列表不能为空。请在配置文件中设置 'services:mqtt:default' 或直接传入 HostList。")
                : new MqttConnection(MqttConnection.CreateClient(options, logger));
        });

        // 注册泛型 MQTT 上下文
        _ = builder.Services.AddSingleton(typeof(IMqttContext<>), typeof(MqttContext<>));

        // 注册发布器
        _ = builder.Services.AddSingleton<IMqttPub, MqttPub>();

        return builder;
    }

    private static readonly Action<ILogger, Exception?> _mqttServerAddressOverridden =
      LoggerMessage.Define(
          LogLevel.Warning,
          new EventId(1, nameof(IMqttConnection)),
          "MQTT服务器地址已被默认配置覆盖，这是预期行为。将使用配置文件中的地址列表。");
}