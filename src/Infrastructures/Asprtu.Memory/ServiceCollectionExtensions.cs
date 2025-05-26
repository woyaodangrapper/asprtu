using Asprtu.Memory.Interprocess.Contracts;
using Asprtu.Memory.Interprocess.Extensions;
using Asprtu.Memory.Interprocess.Queue;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using static Cloudtoid.Contract;

namespace Asprtu.Memory;

/// <summary>
/// Extensions to the <see cref="IServiceCollection"/> to register the shared-memory queue.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers what is needed to create and consume shared-memory queues that are
    /// cross-process accessible.
    /// Use <see cref="IQueueFactory"/> to access the queue.
    /// </summary>
    public static IServiceCollection AddInterprocessQueue([NotNull] this IServiceCollection services)
    {
        _ = CheckValue(services, nameof(services));

        Util.Ensure64Bit();
        services.TryAddSingleton<IQueueFactory, QueueFactory>();
        return services;
    }
}