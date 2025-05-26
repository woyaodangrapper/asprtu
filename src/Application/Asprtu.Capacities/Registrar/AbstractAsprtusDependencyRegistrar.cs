using Asprtu.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Asprtu.Capacities.Registrar;

public abstract partial class AbstractAsprtusDependencyRegistrar : IDependency
{
    public abstract Assembly ApplicationLayerAssembly { get; }

    public abstract IHostApplicationBuilder Builder { get; }

    string IDependency.Name => throw new NotImplementedException();

    /// <summary>
    /// 注册所有服务
    /// </summary>
    public abstract void AddAsprtus();
}