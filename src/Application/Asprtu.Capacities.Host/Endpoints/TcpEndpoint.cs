using Asprtu.Capacities.Contracts;
using Asprtu.Capacities.Host.Contracts;
using Asprtu.Rtu.Contracts;
using Asprtu.Rtu.TcpServer.Contracts;
using Microsoft.AspNetCore.Builder;

namespace Asprtu.Capacities.Host.Endpoints;

public class TcpEndpoint : IEndpoint
{
    private readonly ITcpServerCapacity _tcpServerCapacity;
    private readonly ITcpClientCapacity _tcpClientCapacity;

    public TcpEndpoint(
        ITcpServerCapacity tcpServerCapacity,
        ITcpClientCapacity tcpClientCapacity,
        ILibraryCapacities<ITcpServer> tcpServer
        )
    {
        _tcpServerCapacity = tcpServerCapacity
            ?? throw new ArgumentNullException(nameof(tcpServerCapacity));
        _tcpClientCapacity = tcpClientCapacity
            ?? throw new ArgumentNullException(nameof(tcpClientCapacity));
    }

    public void AddEndpoints(WebApplication app)
    {
        Microsoft.AspNetCore.Routing.RouteGroupBuilder todosApi = app.MapGroup("/tcps");

        _ = todosApi.MapPost("/server/create", () => _tcpServerCapacity.CreateAsync());
        _ = todosApi.MapPost("/client/create", () => _tcpClientCapacity.CreateAsync());

        _ = todosApi.MapDelete("/server/delete", () => _tcpServerCapacity.DeleteAsync());
        _ = todosApi.MapDelete("/client/delete", () => _tcpClientCapacity.DeleteAsync());

        _ = todosApi.MapPut("/server/{id}/redirect/{port}", (string id, string port) =>
            _tcpServerCapacity.RedirectPortAsync(id, port));
        _ = todosApi.MapPut("/client/{id}/redirect/{port}", (string id, string port) =>
            _tcpClientCapacity.RedirectPortAsync(id, port));

        _ = todosApi.MapGet("/client/list", () => _tcpClientCapacity.List());
        _ = todosApi.MapGet("/server/list", () => _tcpServerCapacity.List());
    }
}