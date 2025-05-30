using Microsoft.Extensions.Hosting;

namespace Asprtu.Capacities.Host.Extensions;

public static class OrleansHostBuilderExtensions
{
    /// <summary>
    /// 配置并添加 Orleans Silo 到 IHostBuilder。
    /// </summary>
    /// <param name="builder">IHostBuilder 实例。</param>
    /// <param name="configureSilo">用于配置 ISiloBuilder 的委托。</param>
    /// <returns>配置后的 IHostBuilder。</returns>
    public static IHostBuilder AddOrleansSilo(this IHostBuilder builder, Action<ISiloBuilder>? configureSilo = null)
    {
        return builder
            .ConfigureHostConfiguration(config =>
            {
                //config.AddEnvironmentVariables(prefix: "DOTNET_");
            })
            .UseOrleans(siloBuilder =>
            {
                if (configureSilo == null)
                {
                    _ = siloBuilder.UseLocalhostClustering();
                }
                configureSilo?.Invoke(siloBuilder);
            });
    }
}