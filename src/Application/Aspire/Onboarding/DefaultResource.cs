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

        ConnectionStringExpression = BuildConnectionStringExpression(_options);
    }

    private static ReferenceExpression BuildConnectionStringExpression(OnboardingOptions options)
    {
        // 采用插值字符串处理器（假设 ReferenceExpression 支持）
        ReferenceExpression.ExpressionInterpolatedStringHandler handler = new(24, 3);

        handler.AppendLiteral("Server=");
        handler.AppendFormatted(options.Address);
        handler.AppendLiteral(";Port=");
        handler.AppendFormatted(options.Port);
        handler.AppendLiteral(";Protocol=");
        handler.AppendFormatted(options.Protocol);

        return ReferenceExpression.Create(handler);
    }
}