using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Asprtu.Application.Registrar;

public sealed class DependencyRegistrar(IHostApplicationBuilder builder) : AbstractAsprtusDependencyRegistrar
{
    public override Assembly ApplicationLayerAssembly => Assembly.GetExecutingAssembly();

    public override IHostApplicationBuilder Builder => builder;

    public override void AddAsprtus()
    {
        AddAsprtusCapacityLayer();
    }

}