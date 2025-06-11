using Asprtu.Capacities.Contracts;
using Asprtu.Capacities.Responses;
using Asprtu.Core.Attributes;
using Asprtu.Rtu.Contracts;
using Asprtu.Rtu.Contracts.Tcp;
using Asprtu.Rtu.TcpClient.Contracts;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Asprtu.Capacities.Implementation;

[Asprtus]
public class TcpClientCapacity : AbstractCapacity, ITcpClientCapacity
{
    private readonly ILibraryCapacities<ITcpClient> _tcpClient;

    private readonly ILibraryCapacitiesFactory<ITcpClient> _capacitiesFactory;

    public TcpClientCapacity(

        ILibraryCapacities<ITcpClient> tcpClient,
        ILibraryCapacitiesFactory<ITcpClient> capacitiesFactory
        )
    {
        _tcpClient = tcpClient
            ?? throw new ArgumentNullException(nameof(tcpClient));

        _capacitiesFactory = capacitiesFactory
            ?? throw new ArgumentNullException(nameof(capacitiesFactory));
    }

    private ITcpClient[] All => [.. _capacitiesFactory.All, _tcpClient.Contracts];

    public Task<IResult> CreateAsync() => throw new NotImplementedException();

    public Task<IResult> DeleteAsync() => throw new NotImplementedException();

    public TcpResponse[] List() => [.. All.Select(t => From(t.TcpInfo))];

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