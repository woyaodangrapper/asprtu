namespace Asprtu.Capacities.Contracts;

public interface ITcpClient
{
    Task PistonAsync(CancellationToken stoppingToken);
}