using Asprtu.Capacities.Contracts;
using Asprtu.Capacities.Host.Contracts;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace Asprtu.Capacities.Host.Endpoints;

public class TcpEndpoint([NotNull] ITcpClient tcpClient) : IEndpoint
{
    protected readonly ITcpClient _tcpClient = tcpClient;

    public void AddEndpoints(WebApplication app)
    {
        Microsoft.AspNetCore.Routing.RouteGroupBuilder todosApi = app.MapGroup("/todos");

        _ = todosApi.MapGet("/weather1", () => "Sunny today. 1");
        _ = todosApi.MapGet("/weather2", () => "Sunny today. 2");
        _ = todosApi.MapGet("/weather3", () => "Sunny today. 3");
    }
}