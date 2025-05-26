using Microsoft.AspNetCore.Builder;

namespace Asprtu.Capacities.Host.Contracts;

// Changed the class to an interface and added the 'abstract' modifier to the method declaration
internal interface IEndpoint
{
    void RegisterEndpoints(WebApplication app);
}