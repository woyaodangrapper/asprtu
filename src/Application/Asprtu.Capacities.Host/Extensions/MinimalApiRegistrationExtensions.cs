using Asprtu.Capacities.Host.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asprtu.Capacities.Host.Extensions;

public static class MinimalApiRegistrationExtensions
{
    /// <summary>
    /// 注册所有 IEndpoint 实现类，并调用其 RegisterEndpoints 方法以生成基础 HTTP/1.1 端点，
    /// 提供最基础的 Web API 支持，适用于最低限度的服务发现或上级平台通信。
    /// </summary>
    public static void MapEndpoints(this WebApplication app)
    {
        // 从服务容器中获取所有 IEndpoint
        IEnumerable<IEndpoint> definitions = app.Services.GetServices<IEndpoint>();
        foreach (IEndpoint def in definitions)
        {
            def.RegisterEndpoints(app);
        }
    }

    /// <summary>
    /// 扫描当前程序集中的所有 IEndpoint 实现，
    /// 并将其以 Singleton 方式注入容器，为基础 HTTP/1.1 服务注册做准备。
    /// </summary>
    public static TBuilder AddEndpoints<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        _ = builder.Services.Scan(scan => scan
           .FromAssemblyOf<Program>()
           .AddClasses(classes => classes.AssignableTo<IEndpoint>())
           .AsImplementedInterfaces()
           .WithSingletonLifetime());
        return builder;
    }

    public static TBuilder AddGraphQL<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        _ = builder.Services
         .AddGraphQLServer()
         .AddMyGraphQLTypes()
        .InitializeOnStartup();
        return builder;
    }
}