using Aspire.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Aspire.Onboarding;

internal static class NotifySuperiors
{
    public static INotifySuperiors Create(this DefaultResource resource, ILoggerFactory? loggerFactory = null)

    {
        INotifySuperiors registrar = resource.Protocol.ToLower() switch
        {
            "http" => new HttpRegistrar(new HttpClient(), resource, loggerFactory ?? NullLoggerFactory.Instance),
            _ => throw new NotSupportedException($"Unsupported protocol: {resource.Protocol}")
        };

        return registrar;
    }

    public static INotifySuperiors Create(this IResourceBuilder<DefaultResource> resourceBuilder, ILoggerFactory? loggerFactory = null)
        => resourceBuilder.Resource.Create(loggerFactory);

    /// <summary>
    /// 已初始化时可直接获取实例，否则抛出异常。
    /// </summary>
    public static INotifySuperiors Instance
        => field ?? throw new InvalidOperationException(
            "DependencyRegistrar 未初始化，请先调用 Create()。");
}