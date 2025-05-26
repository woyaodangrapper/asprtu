using Asprtu.Capacities.Host.Contracts;
using Microsoft.AspNetCore.Builder;

namespace Asprtu.Capacities.Host.Endpoints;

public class TcpEndpoint : IEndpoint
{
    public void RegisterEndpoints(WebApplication app)
    {
        Microsoft.AspNetCore.Routing.RouteGroupBuilder todosApi = app.MapGroup("/todos");

        _ = todosApi.MapGet("/weather", () => "Sunny today.");
    }
}