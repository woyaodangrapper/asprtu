using Asprtu.Capacities.Contracts;
using Asprtu.Core.Attributes;
using Asprtu.Rtu.Contracts;

namespace Asprtu.Capacities.Implementation;

[Asprtus]
public class TcpClientCapacity : AbstractCapacity, ITcpClient
{
    protected readonly IEnumerable<ILibraryCapacities> _capacities;

    public TcpClientCapacity(
        IEnumerable<ILibraryCapacities> capacities
        )
    {
        _capacities = capacities
            ?? throw new ArgumentNullException(nameof(capacities));

        foreach (ILibraryCapacities capacity in _capacities)
        {
            if (capacity.Name == "TcpClient" && capacity.Contracts is Rtu.TcpClient.Contracts.ITcpClient tcpClient)
            {
                // Additional logic can be added here if needed
            }
        }
    }

    public Task PistonAsync(CancellationToken stoppingToken) => throw new NotImplementedException();
}