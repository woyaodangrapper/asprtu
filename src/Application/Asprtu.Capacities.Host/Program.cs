using Asprtu.Capacities;
using Asprtu.Capacities.Host.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddHostDefaults().AddAsprtu();


bool inAspire =
    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")?.ToLower() == "true";


// —— 只有在 dev&cli 环境时才去加载 launchSettings.json ——
if ((builder.Environment.IsDevelopment() && Debugger.IsAttached) || !inAspire)
{
    _ = builder.UseLaunchSettings();
    _ = builder.WebHost.UseKestrel();
}

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
    _ = app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => "与gRPC端点的通信必须通过gRPC客户端进行。要了解如何创建客户端，请访问：https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapGrpcService<TcpService>();

app.MapGrpcHealthChecksService();

app.MapDefaultEndpoints();

app.Run();
