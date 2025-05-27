using Aspire.Contracts;
using Aspire.Onboarding;

namespace Aspire.Extensions
{
    internal static class DefaultResourceExtensions
    {
        internal static DefaultResource CreateDefaultResource(this IDistributedApplicationBuilder builder)
        {
            string? config = builder.Configuration["Services:basket:http"];
            if (string.IsNullOrEmpty(config))
            {
                throw new InvalidOperationException("Connection string is not configured.");
            }

            Dictionary<string, string> dict = Util.Parse(config);
            KeyValuePair<string, string> stack = dict.First();

            if (!dict.TryGetValue("port", out string? value) && string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("Port is not configured.");
            }
            OnboardingOptions options = new(stack.Value, stack.Key, value);
            return new(options);
        }

        internal static IResourceBuilder<TDestination> WithReferences<TDestination>(
            this IResourceBuilder<TDestination> builder,
            DefaultResource resource) where TDestination : IResourceWithEnvironment
        {
            return !resource.ServiceUri.IsAbsoluteUri
                ? throw new InvalidOperationException()
                : resource.ServiceUri.AbsolutePath != "/"
                ? throw new InvalidOperationException()
                : builder.WithReference(resource.Name, resource.ServiceUri);
        }
    }
}