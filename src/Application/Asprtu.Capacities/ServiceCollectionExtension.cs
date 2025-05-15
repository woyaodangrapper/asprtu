using Asprtu.Capacities.Registrar;
using Microsoft.Extensions.Hosting;

namespace Asprtu.Capacities;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 添加 Asprtu 远程终端单元(智能硬件)控制功能。
    /// </summary>
    /// <typeparam name="TBuilder">表示 TBuilder 类型，必须实现 IHostApplicationBuilder 接口。</typeparam>
    /// <param name="builder">TBuilder 实例，用于添加 Asprtu 功能。</param>
    /// <returns>返回原始的 TBuilder 实例，支持链式调用。</returns>
    public static TBuilder AddAsprtu<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        new DependencyRegistrar(builder).AddAsprtus();
        return builder;
    }
}