var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Asprtu_Capacities_Host>("grpctcpserverervice");

builder.Build().Run();
