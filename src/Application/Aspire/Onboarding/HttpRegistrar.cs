using Aspire.Contracts;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Aspire.Onboarding;

internal class HttpRegistrar : INotifySuperiors
{
    private readonly HttpClient _httpClient;
    private readonly DefaultResource _resource;
    private readonly ILogger<HttpRegistrar> _logger;

    internal HttpRegistrar(HttpClient httpClient, DefaultResource resource, ILoggerFactory loggerFactory)
    {
        _resource = resource;
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger<HttpRegistrar>();
    }

    public Task<bool> RegisterAsync(CancellationToken cancellationToken = default) => RegisterAsync($"{_resource.ServiceUri}/asprtu/a-{_resource.AppId}/register", cancellationToken);

    public async Task<bool> RegisterAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        var payload = new
        { };
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(requestUri, payload, cancellationToken);
        return response.EnsureSuccessStatusCode().IsSuccessStatusCode;
    }

    public async Task<bool> TryRegisterAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await RegisterAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register with superiors.");
        }
        return false;
    }
}