using Asprtu.Repository.MemoryCache;
using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;

namespace Aspire.EventHub.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCapWithSharedMemory<TSubscriber>(this IServiceCollection services, Action<CapOptions> setupAction, Action<IServiceCollection>? registrarAction = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TSubscriber : class, ICapSubscribe
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
        ArgumentNullException.ThrowIfNull(setupAction, nameof(setupAction));

        //services.AddSingleton<IMemoryCache, SharedMemoryCache>();
        return services.AddInterprocessQueue();
    }
}
