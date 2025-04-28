using Asprtu.Core.Interfaces;

namespace Asprtu.Application.Contracts;

public interface ITcpClient : IAsprtu
{
    Task ProcessClientAsync(CancellationToken stoppingToken);

}
