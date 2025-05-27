using Aspire.Extensions;
using Aspire.Onboarding;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
DefaultResource defaultResource = builder.CreateDefaultResource();
//// 集群模式
//List<Type> types = [];
//HostCapacityExtension.GetProtocolList(types.Add);
//builder.AddProject<Projects.Asprtu_Capacities_Host>("rtu")
//       //.WithEnvironment("RTU_VALUES", JsonSerializer.Serialize(types))
//       .WithReplicas(types.Count);

// 单机模式
//HostCapacityExtension.GetProtocolList((type) =>
//{
//    int port = IPAddressExtensions.GenerateRandomPort();
//    _ = builder.AddProject<Projects.Asprtu_Capacities_Host>(type.Name.ToKebabCase())
//       .WithEndpoint("http", endpoint =>
//       {
//           endpoint.Port = port;
//           endpoint.IsProxied = false;
//       })
//       .WithEndpoint("https", endpoint =>
//       {
//           endpoint.Port = port + 1;
//           endpoint.IsProxied = false;
//       })
//       .WithUrlDocumentation()
//       .WithReferences(defaultResource)
//       .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "True");
//});
builder.AddProject<Projects.Asprtu_Capacities_Host>("hybrid-protocol-stack")
    .WithDocs()
    .WithReferences(defaultResource)
    .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "True");

builder.AddDashboard();
builder.UseStartLogging();
builder.AddDockerComposeEnvironment("compose");

builder.Build().Run();