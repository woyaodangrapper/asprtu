using Aspire.Contracts;
using Aspire.Onboarding;
using Asprtu.Core.Extensions;
using Asprtu.Core.Extensions.Module;

namespace Aspire.Extensions
{
    internal static class DefaultResourceExtensions
    {
        internal static DefaultResource CreateDefaultResource(this IDistributedApplicationBuilder builder)
        {
            string? config = builder.Configuration["basket:http"];
            if (string.IsNullOrEmpty(config))
            {
                throw new InvalidOperationException("Connection string is not configured.");
            }

            Dictionary<string, string> dict = Util.Parse(config);
            KeyValuePair<string, string> stack = dict.First();

            if (!dict.TryGetValue("port", out string? port) && string.IsNullOrEmpty(port))
            {
                throw new InvalidOperationException("Port is not configured.");
            }
            // string host, string type, string port, bool enabled = false, string? name = null
            OnboardingOptions options = new(stack.Value, "basket", port);
            return new(options, stack.Key);
        }

        internal static DefaultResource[] CreateModulesResource(this IDistributedApplicationBuilder builder)
        {
            ModuleProvider moduleProvider = ModuleLoaderExtensions.TryLoad(builder.Configuration);

            List<DefaultResource> resources = [];

            foreach (object module in moduleProvider.All)
            {
                switch (module)
                {
                    case IModule<MqttServerConfig> mqtt:
                        if (!mqtt.Enabled)
                        {
                            continue;
                        }
                        resources.Add(new DefaultResource(mqtt.Config.BrokerUrl.ToString(), mqtt.Type, mqtt.Name));
                        break;

                    case IModule<MqttClientConfig> mqttClient:
                        if (!mqttClient.Enabled)
                        {
                            continue;
                        }
                        resources.Add(new DefaultResource(mqttClient.Config.BrokerUrl.ToString(), mqttClient.Type, mqttClient.Name));
                        break;

                    case IModule<TcpServiceConfig> tcp:
                        if (!tcp.Enabled)
                        {
                            continue;
                        }
                        resources.Add(new DefaultResource(tcp.Config.BrokerUrl.ToString(), tcp.Type, tcp.Name));
                        break;

                    default:
                        break;
                }
            }

            return [.. resources];
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

        internal static IResourceBuilder<TDestination> WithReferences<TDestination>(
       this IResourceBuilder<TDestination> builder,
       DefaultResource[] resource) where TDestination : IResourceWithEnvironment
        {
            for (int i = 0; i < resource.Length; i++)
            {
                _ = builder.WithReferences(resource[i]);
            }

            return builder;
        }
    }
}