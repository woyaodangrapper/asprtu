﻿using Asprtu.gRPC.Capacities;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities.Host.Endpoints;

public class TcpGrpc(ILogger<TcpGrpc> logger) : gRPC.Capacities.TcpGrpc.TcpGrpcBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        logger.LogInformation("The message is received from {Name}", request.Name);
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}