using Aspire.Contracts;
using Aspire.Extensions;
using Aspire.Onboarding;
using Asprtu.Core.Extensions;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

DefaultResource defaultResource = builder.CreateDefaultResource();
DefaultResource[] modulesResource = builder.CreateModulesResource();

ModuleProvider moduleProvider = ModuleLoaderExtensions.TryLoad(builder.Configuration);

MqttOptions options = new(moduleProvider);
IResourceBuilder<ParameterResource> password = builder.AddParameter("password", options.Password);
IResourceBuilder<ParameterResource> username = builder.AddParameter("username", options.Username);
IResourceBuilder<ContainerResource> mqtt = builder.AddNanomq(options.Name)
    .WithHttp(username: username, password: password, port: options.Host[Uri.UriSchemeHttp].Port)
    .WithTcp(options.Host[Uri.UriSchemeNetTcp].Port);

if (options.Dashboard)
{
    _ = builder.AddMonitoring().WaitFor(mqtt);
}
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

builder.AddProject<Projects.Asprtu_Capacities_Host>("asprtu-hybrid")
   .WaitFor(mqtt)
   .WithAPIsDocs("RESTful")
   .WithAPIsDocs("GraphQL", "/graphql")
   .WithReferences(modulesResource)
   .WithReferences(defaultResource)
   .WithEndpoint(name: "tcps", scheme: "tcp", port: 11868, targetPort: 1868)
   .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "True");

builder.AddDashboard();
builder.UseStartLogging();
builder.AddDockerComposeEnvironment("compose");

builder.Build().Run();