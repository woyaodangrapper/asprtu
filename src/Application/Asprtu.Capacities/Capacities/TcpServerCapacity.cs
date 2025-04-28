using Asprtu.Application.Contracts;

namespace Asprtu.Application.Asprtus;

public class TcpServerCapacity : AbstractCapacity, ITcpServer
{
    public Task ProcessServerAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
