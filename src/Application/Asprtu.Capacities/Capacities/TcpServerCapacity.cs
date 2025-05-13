using Asprtu.Application.Contracts;
using Asprtu.Capacities.Capacities;

namespace Asprtu.Application.Asprtus;

public class TcpServerCapacity : AbstractCapacity, ITcpServer
{
    public Task PistonAsync(CancellationToken stoppingToken)
      => throw new NotImplementedException();
}
