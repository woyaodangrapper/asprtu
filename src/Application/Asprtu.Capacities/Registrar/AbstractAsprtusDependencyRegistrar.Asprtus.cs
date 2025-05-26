using Asprtu.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Asprtu.Capacities.Registrar;

public partial class AbstractAsprtusDependencyRegistrar
{
    protected static ReadOnlyCollection<Type> DefaultInterceptorTypes => new(Array.Empty<Type>());

    /// <summary>
    /// 添加协议接口实现到编排容器协议控制台 / 或直接添加到服务
    /// </summary>
    [RequiresUnreferencedCode("This method uses reflection and may break when trimming.")]
    protected virtual void AddAsprtusCapacityLayer()
    {
        ArgumentNullException.ThrowIfNull(ApplicationLayerAssembly, nameof(ApplicationLayerAssembly));

        IEnumerable<Type> exportedTypes = ApplicationLayerAssembly.ExportedTypes;

        foreach (Type type in exportedTypes)
        {
            if (type.IsClass && !type.IsAbstract && type.GetCustomAttribute<AsprtusAttribute>() != null)
            {
                _ = Builder.Services.AddScoped(type);
            }
        }
    }

    /// <summary>
    /// 注册 Application 的IHostedService服务
    /// </summary>
    [RequiresUnreferencedCode("This method uses reflection and may break when trimming.")]
    protected virtual void AddApplicationHostedServices()
    {
        ArgumentNullException.ThrowIfNull(ApplicationLayerAssembly, nameof(ApplicationLayerAssembly));

        IEnumerable<Type> exportedTypes = ApplicationLayerAssembly.ExportedTypes;

        Type serviceType = typeof(IHostedService);

        IEnumerable<Type> implTypes = exportedTypes
            .Where(type =>
                serviceType.IsAssignableFrom(type) &&           // 实现 IHostedService
                !type.IsAbstract &&                             // 排除抽象类
                !type.IsDefined(typeof(NotLoadedAttribute), false)); // 排除带有 NotLoadedAttribute 的类型

        foreach (Type type in implTypes)
        {
            _ = Builder.Services.AddSingleton(serviceType, type);
        }
    }
}