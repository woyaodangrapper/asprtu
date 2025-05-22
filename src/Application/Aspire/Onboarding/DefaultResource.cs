using Aspire.Contracts;

namespace Aspire.Onboarding;

internal class DefaultResource : Resource, IResourceWithConnectionString
{
    private readonly OnboardingOptions _options;

    public readonly Guid AppId = Guid.NewGuid();
    public readonly string Protocol;
    public Uri ServiceUri { get; set; }

    public ReferenceExpression ConnectionStringExpression { get; }

    internal DefaultResource(OnboardingOptions options)
          : base("defaultService")
    {
        _options = options;
        Protocol = _options.Protocol;
        string url = $"{_options.Protocol}://{_options.Address}:{_options.Port}";
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            throw new ArgumentException($"Invalid URL: {url}", nameof(url));
        }
        ServiceUri = uri;

        ConnectionStringExpression = null;
    }
}