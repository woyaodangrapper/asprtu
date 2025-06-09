using Asprtu.Capacities.Responses;
using Microsoft.AspNetCore.Http;

namespace Asprtu.Capacities.Contracts;

public interface ITcpClientCapacity
{
    TcpResponse[] List();

    Task<IResult> CreateAsync();

    Task<IResult> DeleteAsync();

    Task<IResult> RedirectPortAsync(string id, string port);
}