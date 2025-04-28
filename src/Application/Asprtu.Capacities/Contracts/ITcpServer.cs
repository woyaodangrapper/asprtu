using Asprtu.Core.Interfaces;

namespace Asprtu.Application.Contracts;

public interface ITcpServer : IAsprtu
{
    Task ProcessServerAsync(CancellationToken stoppingToken);

}
