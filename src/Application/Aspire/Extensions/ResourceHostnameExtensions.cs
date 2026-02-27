using Aspire.Hosting.Lifecycle;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Aspire.Extensions;

public static class ResourceHostnameExtensions
{
    public static IResourceBuilder<T> WithHostname<T>(this IResourceBuilder<T> builder, string hostname, bool excludeLocalhost = false)
        where T : IResourceWithEndpoints
    {
        builder.ApplicationBuilder.Services.TryAddLifecycleHook<ResourceHostnameLifecycleHook>();
        return builder.WithAnnotation(new HostnameAnnotation([hostname], excludeLocalhost));
    }

    public static IResourceBuilder<T> WithHostname<T>(this IResourceBuilder<T> builder, List<string> hostnames, bool excludeLocalhost = false)
        where T : IResourceWithEndpoints
    {
        builder.ApplicationBuilder.Services.TryAddLifecycleHook<ResourceHostnameLifecycleHook>();
        return builder.WithAnnotation(new HostnameAnnotation(hostnames, excludeLocalhost));
    }

    private class HostnameAnnotation : IResourceAnnotation
    {
        public HostnameAnnotation([EndpointName] List<string> hostnames, bool excludeLocalhost)
        {
            ArgumentNullException.ThrowIfNull(hostnames);

            if (!excludeLocalhost)
            {
                hostnames.Add("localhost");
            }

            this.Hostnames = hostnames;
        }

        public List<string> Hostnames { get; }
    }

    private class ResourceHostnameLifecycleHook(ResourceNotificationService notificationService) : IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            _ = EnsureUrlsAsync();
            return Task.CompletedTask;
        }

        private async Task EnsureUrlsAsync()
        {
            await foreach (var evt in notificationService.WatchAsync())
            {
                if (evt.Snapshot.State != KnownResourceStates.Running || evt.Resource is not IResourceWithEndpoints resource)
                {
                    // By default, .NET Aspire only displays endpoints for running resources.
                    continue;
                }

                var hostnames = evt.Resource.Annotations.OfType<HostnameAnnotation>().SelectMany(a => a.Hostnames).Distinct();
                var endpoints = evt.Resource.Annotations.OfType<EndpointAnnotation>();
                var urlsToAdd = ImmutableArray.CreateBuilder<UrlSnapshot>();

                foreach (var hostname in hostnames)
                {
                    foreach (var endpoint in endpoints)
                    {
                        if (endpoint.AllocatedEndpoint is null)
                        {
                            continue;
                        }

                        var url = $"{endpoint.UriScheme}://{hostname}/port?t={endpoint.AllocatedEndpoint.Port}";
                        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                        {
                            throw new ArgumentException($"'{url}' is not an absolute URL.", nameof(url));
                        }

                        var urlAlreadyAdded = evt.Snapshot.Urls.Any(x => string.Equals(x.Name, hostname, StringComparison.OrdinalIgnoreCase));
                        if (!urlAlreadyAdded)
                        {
                            urlsToAdd.Add(new UrlSnapshot(hostname, url, IsInternal: false));
                        }
                    }
                }

                if (urlsToAdd.Count > 0)
                {
                    await notificationService.PublishUpdateAsync(evt.Resource, snapshot => snapshot with
                    {
                        Urls = urlsToAdd.ToImmutableArray()
                    });

                }
            }
        }
    }
}
