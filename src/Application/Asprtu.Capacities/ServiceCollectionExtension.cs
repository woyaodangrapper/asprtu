using Asprtu.Capacities.EventHub.Mqtt;
using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Asprtu.Capacities.Registrar;
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

    public static IHostApplicationBuilder AddMqtt<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();
        // MqttConnection.Initialize(new(), loggerFactory);
        // MqttConnection.Instance;

        _ = builder.Services.AddSingleton<IMqttConnection>(sp =>
         {
             ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>();
             return new MqttConnection(MqttConnection.CreateClient(new MqttOptions(), loggerFactory.CreateLogger<IMqttConnection>()));
         });

        return builder;
    }
}