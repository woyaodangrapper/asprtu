using Asprtu.Core.Interfaces;

namespace Asprtu.Capacities.Contracts;

public interface ITcpClient : IAsprtu
{
    Task PistonAsync(CancellationToken stoppingToken);
}
