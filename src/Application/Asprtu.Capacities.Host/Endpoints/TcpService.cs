using Asprtu.gRPC.Capacities;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities.Host.Endpoints;

[ApiController]
public class TcpService(ILogger<TcpService> logger) : TcpGrpc.TcpGrpcBase
{

    [HttpGet("sayHello")]
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        logger.LogInformation("The message is received from {Name}", request.Name);

        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }

}