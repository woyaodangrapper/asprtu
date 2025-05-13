namespace Aspire.Extensions
{
    internal static class ResourceBuilderExtensions
    {
        internal static IResourceBuilder<T> WithUrlDocumentation<T>(this IResourceBuilder<T> builder)
            where T : IResource
        {
            return builder.WithUrls(context =>
            {
                context.Urls.ForEach(url =>
                {
                    url.Url += "/docs"; url.DisplayText = $"HTTP/1.1 OpenAPI Docs ({url.Url})";
                });
            });
        }
    }
}
