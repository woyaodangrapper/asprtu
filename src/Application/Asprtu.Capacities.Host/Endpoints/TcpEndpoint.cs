using Asprtu.Capacities.Host.Contracts;
using Microsoft.AspNetCore.Builder;

namespace Asprtu.Capacities.Host.Endpoints;

public class TcpEndpoint : IEndpoint
{
    public void RegisterEndpoints(WebApplication app)
    {
        app.MapGet("/weather", () => "Sunny today.");
    }
}