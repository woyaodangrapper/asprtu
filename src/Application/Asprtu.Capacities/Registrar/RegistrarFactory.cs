using Microsoft.Extensions.Hosting;

namespace Asprtu.Capacities.Registrar;

public static class RegistrarFactory
{
    private static DependencyRegistrar? _instance;

    public static DependencyRegistrar Create(IHostApplicationBuilder builder) => _instance ??= new DependencyRegistrar(builder);

    public static DependencyRegistrar Instance => _instance ?? throw new InvalidOperationException("DependencyRegistrar has not been initialized. Call Create() first.");
}