using Asprtu.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Asprtu.Capacities.Registrar;

public partial class AbstractAsprtusDependencyRegistrar
{
    protected static List<Type> DefaultInterceptorTypes => [];

    /// <summary>
    /// 添加协议接口实现到编排容器协议控制台 / 或直接添加到服务
    /// </summary>
    protected virtual void AddAsprtusCapacityLayer()
    {
        ArgumentNullException.ThrowIfNull(ApplicationLayerAssembly, nameof(ApplicationLayerAssembly));

        if (ApplicationLayerAssembly.ExportedTypes is { } ExportedTypes)
        {
            Type serviceType = typeof(IAsprtu);

            IEnumerable<Type>? asprtuTypes = ApplicationLayerAssembly.ExportedTypes?
                .Where(type => serviceType.IsAssignableFrom(type)
                               && !type.IsInterface
                               && !type.IsAbstract);
            foreach (Type type in asprtuTypes ?? [])
            {
                _ = Builder.Services.AddScoped(typeof(IAsprtu), type);
            }
        }
    }

    /// <summary>
    /// 注册 Application 的IHostedService服务
    /// </summary>
    protected virtual void AddApplicationHostedServices()
    {
        ArgumentNullException.ThrowIfNull(ApplicationLayerAssembly, nameof(ApplicationLayerAssembly));

        if (ApplicationLayerAssembly.ExportedTypes is { } ExportedTypes)
        {
            Type serviceType = typeof(IHostedService);
            Type excludedInterface = typeof(IDiscovery);

            IEnumerable<Type>? implTypes = ExportedTypes
                .Where(type =>
                    serviceType.IsAssignableFrom(type) &&           // 实现 IHostedService
                    !excludedInterface.IsAssignableFrom(type) &&     // 排除 IDiscovery
                    !type.IsAbstract);                              // 排除抽象类

            foreach (Type type in implTypes ?? [])
            {
                _ = Builder.Services.AddSingleton(serviceType, type);
            }
        }
    }
}