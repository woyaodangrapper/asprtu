using Asprtu.Core.Extensions;
using Asprtu.Core.Extensions.Module;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Asprtu.Capacities.Host.Extensions;

public static class LibraryOptionExtensions
{
    public static IHostApplicationBuilder AddLibraryOptions([NotNull] this IHostApplicationBuilder builder)
    {
        ModuleProvider moduleProvider = ModuleLoaderExtensions.TryLoad(builder.Configuration);
        if (moduleProvider.TryGet(out IModule<TcpServiceConfig>? tcpServicConfig) && tcpServicConfig is { } module)
        {
            _ = builder.Services
               .AddSingleton(sp =>
                    new Rtu.TcpServer.Contracts.ChannelOptions(module.Name, module.Config.BrokerUrl.Host, module.Config.BrokerUrl.Port)
               );
        }

        return builder;
    }
}