using Asprtu.Application.Contracts;

namespace Asprtu.Application.Asprtus;

public class TcpClientCapacity : AbstractCapacity, ITcpClient
{
    public Task ProcessClientAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
