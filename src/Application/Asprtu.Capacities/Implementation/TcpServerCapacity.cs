using Asprtu.Capacities.Contracts;
using Asprtu.Core.Attributes;

namespace Asprtu.Capacities.Implementation;

[Asprtus]
public class TcpServerCapacity : AbstractCapacity, ITcpServer
{
    public Task PistonAsync(CancellationToken stoppingToken) => throw new NotImplementedException();
}