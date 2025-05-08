using Microsoft.Extensions.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.   


//// 集群模式
//List<Type> types = [];
//HostCapacityExtension.GetProtocolList(types.Add);
//builder.AddProject<Projects.Asprtu_Capacities_Host>("rtu")
//       .WithEnvironment("RTU_VALUES", JsonSerializer.Serialize(types))
//       .WithReplicas(types.Count);

// 单机模式
HostCapacityExtension.GetProtocolList((type) =>
{
    _ = builder.AddProject<Projects.Asprtu_Capacities_Host>(type.Name)
        .WithEndpoint("https", endpoint => endpoint.IsProxied = false)
        .WithEnvironment("RTU_VALUES", "TCP");
});

builder.Build().Run();
