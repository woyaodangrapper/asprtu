using System.Reflection;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class HostCapacityExtension
{
    //public static IDistributedApplicationBuilder AddTcpServer(this IDistributedApplicationBuilder builder, string name)
    //{
    //    return builder.AddResource(new TcpServerResource(name))
    //                  .WithImage("application")
    //                  .WithHttpEndpoint(targetPort: 8080);
    //}

    //public static IDistributedApplicationBuilder AddTcpClient(this IDistributedApplicationBuilder builder, string name)
    //{
    //    return builder.AddResource(new TcpClientResource(name))
    //                  .WithImage("application")
    //                  .WithHttpEndpoint(targetPort: 8081);
    //}

    public static void GetProtocolList(Action<Type> action)
    {
        AssemblyName[] referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
        IEnumerable<Assembly> assemblys = referencedAssemblies.Select(assembly => Assembly.Load(assembly.Name));
        RTU.Infrastructures.Extensions.Util.GetProtocolList(action, assemblys);
    }
}