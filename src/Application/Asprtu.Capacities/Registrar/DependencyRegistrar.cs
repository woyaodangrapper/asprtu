using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Asprtu.Capacities.Registrar;

public sealed class DependencyRegistrar(IHostApplicationBuilder builder) : AbstractAsprtusDependencyRegistrar
{
    public override Assembly ApplicationLayerAssembly => Assembly.GetExecutingAssembly();

    public override IHostApplicationBuilder Builder => builder;

    public override void AddAsprtus()
    {
        AddAsprtusCapacityLayer();
        AddApplicationHostedServices();
    }

    public IHostApplicationBuilder UseAsprtus()
    {
        AddAsprtus();
        return Builder;
    }
}