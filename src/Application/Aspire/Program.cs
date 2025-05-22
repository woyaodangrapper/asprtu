using Aspire.Extensions;
using Aspire.Onboarding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RTU.Infrastructures.Extensions;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

DefaultResource defaultResource = builder.CreateDefaultResource();
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
       .WithUrlDocumentation()
       .WithReferences(defaultResource)
       .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "True");
});

Aspire.Contracts.INotifySuperiors Notify = defaultResource.Create();

// Source: https://docs.microsoft.com/en-us/dotnet/api/system.string.contains
// This method is based on the official .NET documentation example.
builder.Eventing.Subscribe<BeforeStartEvent>(
    static (@event, cancellationToken) =>
    {
        ILogger<Program> logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("1. 开始事件之前");

        return Task.CompletedTask;
    });

builder.Eventing.Subscribe<AfterEndpointsAllocatedEvent>(
    static (@event, cancellationToken) =>
    {
        ILogger<Program> logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("2. 终点分配事件后");

        return Task.CompletedTask;
    });

builder.Eventing.Subscribe<AfterResourcesCreatedEvent>(
     async (@event, cancellationToken) =>
    {
        ILogger<Program> logger = @event.Services.GetRequiredService<ILogger<Program>>();

        _ = await Notify.TryRegisterAsync(cancellationToken);

        logger.LogInformation("3. 资源创建事件之后");
    });

builder.Build().Run();