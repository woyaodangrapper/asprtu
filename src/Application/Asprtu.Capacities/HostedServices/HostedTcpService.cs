using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Asprtu.Core.Extensions;
using Asprtu.Core.Extensions.Module;
using Asprtu.Rtu.Contracts;
using Asprtu.Rtu.TcpServer.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities.HostedServices;

public class HostedTcpService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HostedTcpService> _logger;
    private readonly ITcpServer _tcpServer;
    private readonly IMqttPub _mqttPub; // 仅用于测试，建议添加轮询器在内置队列中分发到不同的 MQTT 主题。

    public HostedTcpService(
        ILogger<HostedTcpService> logger,
        IConfiguration configuration,
        IMqttPub mqttPub,
        ILibraryCapacities<ITcpServer> capacities
        )
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(capacities);

        _logger = logger;
        _mqttPub = mqttPub;
        _configuration = configuration;
        _tcpServer = capacities.Contracts;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ModuleProvider moduleProvider = ModuleLoaderExtensions.TryLoad(_configuration);
        if (moduleProvider.TryGet(out IModule<TcpServiceConfig>? tcpServicConfig) && tcpServicConfig is { Enabled: true } module)
        {
            _tcpServer.OnMessage = async (_, _, message) =>
            {
                if (await _mqttPub.TryPublishAsync(message))
                {
                    _logger.LogInformation("Message published successfully: {Message}", message);
                }
                else
                {
                    _logger.LogWarning("Failed to publish message: {Message}", message);
                }
            };
            _tcpServer.OnSuccess = (_) => _logger.LogDebug("OnSuccess");
            _tcpServer.OnError = (_) => _logger.LogDebug("OnError");

            await _tcpServer.TryExecuteAsync().ConfigureAwait(false);
        }
    }
}