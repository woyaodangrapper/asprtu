using Aspire.Extensions;
using Microsoft.Extensions.Hosting;
using RTU.Infrastructures.Extensions;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

//// 集群模式
//List<Type> types = [];
//HostCapacityExtension.GetProtocolList(types.Add);
//builder.AddProject<Projects.Asprtu_Capacities_Host>("rtu")
//       //.WithEnvironment("RTU_VALUES", JsonSerializer.Serialize(types))
//       .WithReplicas(types.Count);

// 单机模式
HostCapacityExtension.GetProtocolList((type) =>
{
    int port = IPAddressExtensions.GenerateRandomPort();
    _ = builder.AddProject<Projects.Asprtu_Capacities_Host>(type.Name.ToKebabCase())
        .WithEndpoint("http", endpoint =>
        {
            endpoint.Port = port;
            endpoint.IsProxied = false;
        })
        .WithEndpoint("https", endpoint =>
        {
            endpoint.Port = port + 1;
            endpoint.IsProxied = false;
        })
        //.WithEndpoint(name: "docs", scheme: "http")
        .WithUrlDocumentation()
        .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "True");
});


builder.Build().Run();
