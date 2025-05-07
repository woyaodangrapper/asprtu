using Microsoft.Extensions.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.   

HostCapacityExtension.GetProtocolList((type) =>
{
    _ = builder.AddProject<Projects.Asprtu_Capacities_Host>(type.Name)
        .WithEnvironment("RTU_VALUE", "TCP");
});


builder.Build().Run();
