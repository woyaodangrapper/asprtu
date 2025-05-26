using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Asprtu.Capacities.Host.Extensions;

public static class SwaggerExtensions
{
    /// <summary>
    /// 配置 OpenAPI 文档提供第三方或集成中心。
    /// </summary>
    /// <typeparam name="TBuilder">应用程序构建器的类型，必须实现 <see cref="IHostApplicationBuilder"/>。</typeparam>
    /// <param name="builder">应用程序构建器实例。</param>
    public static TBuilder AddApiDocumentation<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        _ = builder.Services.AddOpenApi()
           .AddGrpcSwagger()
           .AddSwaggerGen()
           .AddEndpointsApiExplorer();
        return builder;
    }

    /// <summary>
    /// 启用 OpenAPI UI
    /// </summary>
    /// <param name="app">Web 应用程序实例。</param>
    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        _ = app.MapOpenApi();
        _ = app.MapScalarApiReference(endpointPrefix: "docs");
        return app;
    }
}