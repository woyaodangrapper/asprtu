using Aspire.Contracts;

namespace Aspire.Onboarding;

internal class DefaultResource : Resource, IResourceWithConnectionString
{
    private readonly OnboardingOptions _options;

    public readonly Guid AppId = Guid.NewGuid();
    public Uri ServiceUri { get; set; }

    public ReferenceExpression ConnectionStringExpression { get; }

    internal DefaultResource(string url, string type, string name, bool enabled = false)
      : base(name)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            throw new ArgumentException($"Invalid URL: {url}", nameof(url));
        }
        ServiceUri = uri;
        _options = new OnboardingOptions(uri.Host, type, uri.Port.ToString(), enabled, name);
        ConnectionStringExpression = BuildConnectionStringExpression(_options, uri.Scheme);
    }

    internal DefaultResource(OnboardingOptions options, string scheme)
          : base(options.Name)
    {
        _options = options;
        string url = $"{scheme}://{options.Host}:{_options.Port}";
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            throw new ArgumentException($"Invalid URL: {url}", nameof(url));
        }
        ServiceUri = uri;
        ConnectionStringExpression = BuildConnectionStringExpression(_options, scheme);
    }

    private static ReferenceExpression BuildConnectionStringExpression(OnboardingOptions options, string scheme)
    {
        // 采用插值字符串处理器（假设 ReferenceExpression 支持）
        ReferenceExpression.ExpressionInterpolatedStringHandler handler = new(24, 3);

        handler.AppendLiteral("Server=");
        handler.AppendFormatted(options.Host);
        handler.AppendLiteral(";Port=");
        handler.AppendFormatted(options.Port);
        handler.AppendLiteral(";Protocol=");
        handler.AppendFormatted(scheme);

        return ReferenceExpression.Create(handler);
    }
}