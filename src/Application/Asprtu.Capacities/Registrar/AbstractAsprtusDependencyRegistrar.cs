using Asprtu.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Asprtu.Application.Registrar;

public abstract partial class AbstractAsprtusDependencyRegistrar : IDependency
{
    public string Name => "asprtus";

    public abstract Assembly ApplicationLayerAssembly { get; }

    public abstract IHostApplicationBuilder Builder { get; }

    /// <summary>
    /// 注册所有服务
    /// </summary>
    public abstract void AddAsprtus();
}