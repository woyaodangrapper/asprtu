using Asprtu.Capacities.EventHub.Mqtt.Contracts;
using Asprtu.Capacities.EventHub.Mqtt.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;
using System.Text;

namespace Asprtu.Capacities.EventHub.Mqtt;

public sealed class MqttConnection : IMqttConnection
{
    private static MqttOptions? _options;
    private static ILogger<IMqttConnection>? _logger;

    private static readonly Lazy<MqttConnection> _lazyInstance = new(() =>
    {
        return _options == null || _logger == null
            ? throw new InvalidOperationException("MqttConnection is not initialized. Call Initialize() first.")
            : new MqttConnection
            {
                Connection = CreateClient(_options, _logger)
            };
    }, isThreadSafe: true);

    public static MqttConnection Instance => _lazyInstance.Value;

    public static void Initialize(MqttOptions options, ILoggerFactory? loggerFactory = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (_options != null)
        {
            throw new InvalidOperationException("MqttConnection is already initialized.");
        }

        _options = options;
        _logger = (loggerFactory ?? NullLoggerFactory.Instance).CreateLogger<MqttConnection>();
    }

    private MqttConnection()
    { }

    public MqttConnection(IMqttClient connection) => Connection = connection;

    public IMqttClient Connection { get; set; } = new MqttClientFactory().CreateMqttClient();

    public static IMqttClient CreateClient([NotNull] MqttOptions options, [NotNull] ILogger logger, TimeSpan? reconnectInterval = null)
    {
        reconnectInterval ??= TimeSpan.FromSeconds(5);

        MqttClientFactory factory = new();
        IMqttClient client = factory.CreateMqttClient();
        Notification(client, logger, options.ClientId);

        MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
            .WithClientId(options.ClientId ?? Guid.NewGuid().ToString("N"))
            .WithCleanSession(options.CleanSession);

        if (Util.ToMqttEndpoints(options.HostList) is { } endpoints && endpoints.Any())
        {
            foreach ((Uri uri, string type, string host, int port, string? path) in endpoints)
            {
                builder = type switch
                {
                    "tcp" => builder.WithTcpServer(host, port),
                    "ws" or "wss" => builder.WithWebSocketServer(wsOpts => wsOpts.WithUri(uri.ToString())),
                    _ => throw new FormatException($"无效协议头格式: '{type}'"),
                };
            }
        }

        if (!string.IsNullOrEmpty(options.Username))
        {
            builder = builder.WithCredentials(options.Username, options.Password ?? string.Empty);
        }

        if (options.UseTls)
        {
            builder = builder.WithTlsOptions(o =>
            {
                _ = o.WithCertificateValidationHandler(_ => true);
                _ = o.WithSslProtocols(SslProtocols.None);
            });
        }

        MqttClientOptions clientOptions = builder.Build();
        try
        {
            _ = client.ConnectAsync(clientOptions).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Util.LogErrorMessage(logger, "MQTT initial ConnectAsync threw exception: {Message}", ex);
            throw;
        }

        return client;
    }

    /// <summary>
    /// MQTT 客户端状态通知（连接、断开、接收消息等）。
    /// </summary>
    /// <param name="client"></param>
    /// <param name="logger"></param>
    /// <param name="clientId"></param>
    private static void Notification([NotNull] IMqttClient client, ILogger logger, string? clientId)
    {
        client.ConnectedAsync += async e =>
        {
            Util.LogInformationMessage(logger, $"MQTT connected successfully (ClientId={clientId}).", null);
            await Task.CompletedTask.ConfigureAwait(false);
        };

        client.DisconnectedAsync += async e =>
        {
            string reason = e.ReasonString ?? e.Reason.ToString();
            Util.LogWarningMessage(logger, $"MQTT disconnected. Reason: {reason}.", null);
            await Task.CompletedTask.ConfigureAwait(false);
        };

        client.ApplicationMessageReceivedAsync += async e =>
        {
            string payload = e.ApplicationMessage?.Payload is { Length: > 0 }
                ? Encoding.UTF8.GetString(e.ApplicationMessage.Payload)
                : string.Empty;
            Util.LogInformationMessage(logger, $"MQTT received message on topic {e.ApplicationMessage?.Topic}: {payload}", null);
            await Task.CompletedTask.ConfigureAwait(false);
        };
    }

    public void Dispose() => Connection.Dispose();
}