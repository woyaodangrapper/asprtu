namespace Aspire.Contracts;

internal class OnboardingOptions
{
    internal OnboardingOptions(string address, string protocol, string port)
    {
        Address = address;
        Protocol = protocol;
        Port = port;
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public string Address { get; set; }
    public string Protocol { get; set; }
    public string Port { get; set; }
    public long Timestamp { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Address))
            throw new ArgumentException("Address 不能为空", nameof(Address));
        if (string.IsNullOrWhiteSpace(Protocol))
            throw new ArgumentException("Protocol 不能为空", nameof(Protocol));
    }
}