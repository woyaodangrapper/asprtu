namespace Aspire.Extensions
{
    internal static class ResourceBuilderExtensions
    {
        internal static IResourceBuilder<T> WithUrlDocumentation<T>(this IResourceBuilder<T> builder)
            where T : IResourceWithEndpoints
        {
            return builder.WithEndpoint(name: "docs", scheme: "http").WithUrls(context =>
            {

                ResourceUrlAnnotation? annotation = context.Urls.SingleOrDefault((url)
                    => StringComparer.OrdinalIgnoreCase.Equals("docs", url.Endpoint?.EndpointName
                 ));

                if (annotation is { } endpoint)
                {
                    endpoint.Url += "/docs";
                    endpoint.DisplayText = $"Project (docs)";
                }

            });
        }
    }
}
