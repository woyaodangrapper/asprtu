using Asprtu.Application.Contracts;

namespace Asprtu.Capacities.Implementation;

public class TcpServerCapacity : AbstractCapacity, ITcpServer
{
    public Task PistonAsync(CancellationToken stoppingToken)
      => throw new NotImplementedException();
}
