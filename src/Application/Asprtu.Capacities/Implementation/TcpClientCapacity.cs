using Asprtu.Capacities.Contracts;
using Asprtu.Core.Attributes;

namespace Asprtu.Capacities.Implementation;

[Asprtus]
public class TcpClientCapacity : AbstractCapacity, ITcpClient
{
    public Task PistonAsync(CancellationToken stoppingToken) => throw new NotImplementedException();
}