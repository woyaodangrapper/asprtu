using Asprtu.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Asprtu.Capacities.Registrar;

partial class AbstractAsprtusDependencyRegistrar
{
    protected static List<Type> DefaultInterceptorTypes => [];

    /// <summary>
    /// 添加协议接口实现到编排容器协议控制台 / 或直接添加到服务
    /// </summary>
    protected virtual void AddAsprtusCapacityLayer()
    {
        //var serviceType = typeof(IAsprtu);
        //var implTypes = ApplicationLayerAssembly.ExportedTypes.Where(type => type.IsAssignableTo(serviceType) && type.IsNotAbstractClass(true)).ToList();
        //implTypes.ForEach(asprtuType =>
        //{
        //    var implType = ApplicationLayerAssembly.ExportedTypes.FirstOrDefault(type => type.IsAssignableTo(asprtuType) && type.IsNotAbstractClass(true));
        //    if (implType is null)
        //        return;
        //    Builder.Services.AddScoped(asprtuType, provider =>
        //    {
        //        var interfaceToProxy = asprtuType;
        //        var target = provider.GetService(implType);
        //        var interceptors = DefaultInterceptorTypes.ConvertAll(interceptorType => provider.GetService(interceptorType) as IInterceptor).ToArray();
        //        var proxyGenerator = provider.GetService<ProxyGenerator>();
        //        var proxy = proxyGenerator.CreateInterfaceProxyWithTargetInterface(interfaceToProxy, target, interceptors);
        //        return proxy;
        //    });
        //});


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
