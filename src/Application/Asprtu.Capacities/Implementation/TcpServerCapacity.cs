using Asprtu.Capacities.Contracts;
using Asprtu.Capacities.Responses;
using Asprtu.Core.Attributes;
using Asprtu.Rtu.Contracts;
using Asprtu.Rtu.Contracts.Tcp;
using Asprtu.Rtu.TcpServer.Contracts;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Asprtu.Capacities.Implementation;

[Asprtus]
public class TcpServerCapacity : AbstractCapacity, ITcpServerCapacity
{
    protected readonly ILibraryCapacities<ITcpServer> _tcpServer;

    protected readonly ILibraryCapacitiesFactory<ITcpServer> _capacitiesFactory;

    public TcpServerCapacity(
        ILibraryCapacities<ITcpServer> tcpServer,
        ILibraryCapacitiesFactory<ITcpServer> capacitiesFactory
        )
    {
        _tcpServer = tcpServer
            ?? throw new ArgumentNullException(nameof(tcpServer));

        _capacitiesFactory = capacitiesFactory
            ?? throw new ArgumentNullException(nameof(capacitiesFactory));
    }

    private ITcpServer[] All => [.. _capacitiesFactory.All, _tcpServer.Contracts];

    public Task<IResult> CreateAsync() => throw new NotImplementedException();

    public Task<IResult> DeleteAsync() => throw new NotImplementedException();

    public TcpResponse[] List() => [.. All.Select(t => From(t.TcpInfo))];

    public Task PistonAsync(CancellationToken stoppingToken) => throw new NotImplementedException();

    public Task<IResult> RedirectPortAsync(string id, string port) => throw new NotImplementedException();

    private static TcpResponse From([NotNull] TcpInfo info)
    {
        return new TcpResponse
        {
            Id = info.ConnectionId.ToString(),
            Name = string.IsNullOrWhiteSpace(info.ClientAppName) ? "Unknown" : info.ClientAppName,
            Port = info.LocalEndPoint?.Port.ToString() ?? "N/A",
            Protocol = info.Version,
            IsRunning = info.State == ConnectionState.Active,
            CreatedAt = new DateTimeOffset(info.ConnectedAt, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(info.LastActivityAt, TimeSpan.Zero)
        };
    }
}