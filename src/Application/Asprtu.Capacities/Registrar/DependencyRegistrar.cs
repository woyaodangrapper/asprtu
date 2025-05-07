using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Asprtu.Capacities.Registrar;

public sealed class DependencyRegistrar(IHostApplicationBuilder builder) : AbstractAsprtusDependencyRegistrar
{
    public override Assembly ApplicationLayerAssembly => Assembly.GetExecutingAssembly();

    public override IHostApplicationBuilder Builder => builder;

    public override void AddAsprtus()
    {
        _ = Builder.Services.AddQueueFactory<byte[]>("TCP");
        _ = Builder.Services.AddTcpServerFactory();
        _ = Builder.Services.AddTcpClientFactory();
        _ = Builder.Services.AddProtocolManifest();
        AddAsprtusCapacityLayer();
    }
}