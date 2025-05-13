using Asprtu.Capacities.Capacities;
using Asprtu.Capacities.Contracts;

namespace Asprtu.Application.Asprtus;

public class TcpClientCapacity : AbstractCapacity, ITcpClient
{
    public Task PistonAsync(CancellationToken stoppingToken)
      => throw new NotImplementedException();
}
