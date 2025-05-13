using Asprtu.Capacities.Contracts;

namespace Asprtu.Capacities.Implementation;

public class TcpClientCapacity : AbstractCapacity, ITcpClient
{
    public Task PistonAsync(CancellationToken stoppingToken)
      => throw new NotImplementedException();
}
