namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder UseGrpc(this IApplicationBuilder application)
    {

        //var serviceInfo = application.ApplicationServices.GetRequiredService<IServiceInfo>();
        //var middlewareRegistarType = serviceInfo.StartAssembly.ExportedTypes.FirstOrDefault(m => m.IsAssignableTo(typeof(IMiddlewareRegistrar)) && m.IsNotAbstractClass(true));
        //if (middlewareRegistarType is null)
        //    throw new NullReferenceException(nameof(IMiddlewareRegistrar));

        //var middlewareRegistar = Activator.CreateInstance(middlewareRegistarType, app) as IMiddlewareRegistrar;
        //middlewareRegistar?.UseAdnc();

        return application;
    }

}
