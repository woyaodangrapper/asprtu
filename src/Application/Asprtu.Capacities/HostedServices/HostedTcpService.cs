using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Asprtu.Capacities.EventHub.Mqtt.Messages;
using Asprtu.Core.Extensions;
using Asprtu.Core.Extensions.Module;
using Asprtu.Rtu.Contracts;
using Asprtu.Rtu.TcpServer.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Asprtu.Capacities.HostedServices;

public class HostedTcpService : BackgroundService
{
    private readonly IConfiguration _configuration;

    //private readonly ILogger<HostedTcpService> _logger;
    private readonly ITcpServer _tcpServer;

    private readonly IMqttContext<HellMessage> _context;

    public HostedTcpService(
        //ILogger<HostedTcpService> logger,
        IConfiguration configuration,
        IMqttContext<HellMessage> context,
        ILibraryCapacities<ITcpServer> capacities
        )
    {
        //ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(capacities);

        //_logger = logger;
        _context = context;
        _configuration = configuration;
        _tcpServer = capacities.Contracts;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ModuleProvider moduleProvider = ModuleLoaderExtensions.TryLoad(_configuration);
        if (moduleProvider.TryGet(out IModule<TcpServiceConfig>? tcpServicConfig) && tcpServicConfig is { Enabled: true } module)
        {
            _tcpServer.OnMessage = (_, _, message) => _context.Add(new HellMessage());
            //_tcpServer.OnSuccess = (_) => _logger.LogDebug("OnSuccess");
            //_tcpServer.OnError = (_) => _logger.LogDebug("OnError");

            await _tcpServer.TryExecuteAsync().ConfigureAwait(false);
        }
    }
}