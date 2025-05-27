using Aspire.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Aspire.Onboarding;

internal static class NotifySuperiors
{
    private static readonly Action<ILogger, string, Exception?> LogMessage =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(Program)),
            "The message is received from {Name}");

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

    public static void UseStartLogging(
        this IDistributedApplicationBuilder builder,
        Func<IServiceProvider, CancellationToken, Task>? afterResourcesCreatedCallback = null)
    {
        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.string.contains
        // This method is based on the official .NET documentation example.
        _ = builder.Eventing.Subscribe<BeforeStartEvent>(
            static (@event, cancellationToken) =>
            {
                ILogger<Program> logger = @event.Services.GetRequiredService<ILogger<Program>>();
                LogMessage(logger, "BeforeStartEvent triggered", null);
                return Task.CompletedTask;
            });

        _ = builder.Eventing.Subscribe<AfterEndpointsAllocatedEvent>(
            static (@event, cancellationToken) =>
            {
                ILogger<Program> logger = @event.Services.GetRequiredService<ILogger<Program>>();
                LogMessage(logger, "Endpoints have been allocated", null);

                return Task.CompletedTask;
            });

        _ = builder.Eventing.Subscribe<AfterResourcesCreatedEvent>(
            async (@event, cancellationToken) =>
            {
                ILogger<Program> logger = @event.Services.GetRequiredService<ILogger<Program>>();
                LogMessage(logger, "Resources created successfully", null);

                if (afterResourcesCreatedCallback is not null)
                {
                    await afterResourcesCreatedCallback.Invoke(@event.Services, cancellationToken);
                }
            });
    }

    /// <summary>
    /// 已初始化时可直接获取实例，否则抛出异常。
    /// </summary>
    public static INotifySuperiors Instance
        => field ?? throw new InvalidOperationException(
            "DependencyRegistrar 未初始化，请先调用 Create()。");
}