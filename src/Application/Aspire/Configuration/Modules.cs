namespace Aspire.Configuration;

public record MqttServerConfig(Uri BrokerUrl, string? Image = null);
public record MqttClientConfig(Uri BrokerUrl, string ClientId);
public record TcpServiceConfig(Uri BrokerUrl);
public record MqttServerModule : IModule<MqttServerConfig>
{
    public string Name { get; init; } = "mqtt-server";
    public string Type { get; init; } = string.Empty;
    public bool Enabled { get; init; }
    public MqttServerConfig Config { get; init; } = new(new Uri("mqtt://localhost:1883"));
}

public record MqttClientModule : IModule<MqttClientConfig>
{
    public string Name { get; init; } = "mqtt-client";
    public string Type { get; init; } = string.Empty;
    public bool Enabled { get; init; }
    public MqttClientConfig Config { get; init; } = new(new Uri("mqtt://localhost:1883"), "mqtt_client_id");
}
public record TcpServiceModule : IModule<TcpServiceConfig>
{
    public string Name { get; init; } = "tcp-service";
    public string Type { get; init; } = string.Empty;
    public bool Enabled { get; init; }
    public TcpServiceConfig Config { get; init; } = new(new Uri("tcp://localhost:1868"));
}