namespace Asprtu.Capacities.Contracts;

public interface ITcpServer
{
    Task PistonAsync(CancellationToken stoppingToken);
}