using Asprtu.Rtu.Contracts;
using Asprtu.Rtu.TcpServer.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities;

public class test : BackgroundService
{
    public test(ILogger<test> logger,
        ILibraryCapacities<ITcpServer> capacities
        )
    {
        _logger = logger;
        _tcpServer = capacities!.Contracts;
    }

    private readonly ILogger<test> _logger;
    private readonly ITcpServer _tcpServer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _tcpServer.OnMessage = (_, _, message) =>
        {
            _logger.LogDebug("megsage received: {Message}", message);
        };

        _tcpServer.OnSuccess = (_) => _logger.LogDebug("OnSuccess");
        _tcpServer.OnError = (_) => _logger.LogDebug("OnError");

        await _tcpServer.TryExecuteAsync().ConfigureAwait(false);
    }
}