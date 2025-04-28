
using Asprtu.Application;
using Asprtu.Capacities.Host.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddHostDefaults().AddAsprtu();
//builder.add();

var app = builder.Build();

app.MapGet("/", () => "与gRPC端点的通信必须通过gRPC客户端进行。要了解如何创建客户端，请访问：https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapGrpcService<TcpService>();



app.MapGrpcHealthChecksService();

app.MapDefaultEndpoints();

app.Run();
