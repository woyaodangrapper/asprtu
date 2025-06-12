namespace Asprtu.Core.Extensions.Module;

public readonly struct Hosts : IEquatable<Hosts>
{
    private static readonly Dictionary<string, Uri> _schemes = new(StringComparer.OrdinalIgnoreCase);

    public Hosts Add(string scheme, string host = "localhost", int port = 0)
    {
        UriBuilder builder = new(scheme, host)
        {
            Port = port
        };
        _schemes[scheme] = builder.Uri;
        return this;
    }

    public readonly Uri this[string scheme] => _schemes[scheme];

    public override bool Equals(object? obj) => obj is Hosts other && Equals(other);

    public bool Equals(Hosts other) => _schemes.SequenceEqual(_schemes);

    public override int GetHashCode() => _schemes.GetHashCode();

    public static bool operator ==(Hosts left, Hosts right) => left.Equals(right);

    public static bool operator !=(Hosts left, Hosts right) => !(left == right);
}