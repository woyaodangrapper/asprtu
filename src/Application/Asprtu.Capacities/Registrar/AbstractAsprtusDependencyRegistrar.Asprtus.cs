using Asprtu.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Asprtu.Capacities.Registrar;

public partial class AbstractAsprtusDependencyRegistrar
{
    protected static List<Type> DefaultInterceptorTypes => [];

    /// <summary>
    /// 添加协议接口实现到编排容器协议控制台 / 或直接添加到服务
    /// </summary>
    protected virtual void AddAsprtusCapacityLayer()
    {

        Type serviceType = typeof(IAsprtu);

        IEnumerable<Type>? asprtuTypes = ApplicationLayerAssembly.ExportedTypes?
            .Where(type => serviceType.IsAssignableFrom(type)
                           && !type.IsInterface
                           && !type.IsAbstract);
        foreach (Type type in asprtuTypes ?? [])
        {
            Builder.Services.AddScoped(typeof(IAsprtu), type);
        }

    }
}
