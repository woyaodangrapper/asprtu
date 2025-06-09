namespace Aspire.Extensions
{
    internal static class ResourceBuilderExtensions
    {
        /// <summary>
        /// 为资源添加一个 API 文档端点，方便通过 HTTP 访问项目文档。
        /// </summary>
        /// <typeparam name="T">实现了 <see cref="IResourceWithEndpoints"/> 的资源类型。</typeparam>
        /// <param name="builder">资源构建器实例。</param>
        /// <param name="apiType">API 类型描述，如 "RESTful" 或 "GraphQL"。</param>
        /// <param name="docsPath">文档路径，默认是 "/docs"。GraphQL 可传 "/graphql" 或 "/graphql/ui"。</param>
        /// <returns>带有指定文档端点配置的资源构建器。</returns>
        internal static IResourceBuilder<T> WithAPIsDocs<T>(
            this IResourceBuilder<T> builder,
            string apiType,
            string docsPath = "/docs")
            where T : IResourceWithEndpoints
        {
            // 端点名称根据apiType动态生成，防止冲突
            string endpointName = apiType.ToUpperInvariant() + "-docs";

            return builder.WithEndpoint(name: endpointName, scheme: "http").WithUrls(context =>
            {
                ResourceUrlAnnotation? annotation = context.Urls.SingleOrDefault(url =>
                    StringComparer.OrdinalIgnoreCase.Equals(endpointName, url.Endpoint?.EndpointName));

                if (annotation is { } endpoint)
                {
                    endpoint.Url += docsPath;
                    endpoint.DisplayText = $"Project ({apiType} docs)";
                }
            });
        }
    }
}