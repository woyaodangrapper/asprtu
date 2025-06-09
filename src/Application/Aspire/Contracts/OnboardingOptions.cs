namespace Aspire.Contracts;

internal class OnboardingOptions
{
    internal OnboardingOptions(string host, string type, string port, string? name = null)
    {
        Name = name ?? "defaultService";
        Port = port;
        Type = type;
        Host = host;
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public string Name { get; set; }
    public string Type { get; set; }
    public string Host { get; set; }
    public string Port { get; set; }
    public long Timestamp { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Port))
        {
            throw new ArgumentException("Port 不能为空", nameof(Port));
        }

        if (string.IsNullOrWhiteSpace(Host))
        {
            throw new ArgumentException("Host 不能为空", nameof(Host));
        }

        if (string.IsNullOrWhiteSpace(Type))
        {
            throw new ArgumentException("Type 不能为空", nameof(Type));
        }
    }
}