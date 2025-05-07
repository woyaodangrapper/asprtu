using DotNetCore.CAP.Filter;
using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities.EventHub.Cap.Filters;

/// <summary>
/// https://cap.dotnetcore.xyz/user-guide/zh/cap/filter/
/// </summary>
public sealed class DefaultCapFilter(ILogger<DefaultCapFilter> logger) : SubscribeFilter
{
    public override Task OnSubscribeExecutingAsync(ExecutingContext context)
    {
        return Task.CompletedTask;
    }

    public override Task OnSubscribeExecutedAsync(ExecutedContext context)
    {
        return Task.CompletedTask;
    }

    public override Task OnSubscribeExceptionAsync(ExceptionContext context)
    {
        logger.LogError(context.Exception, "OnSubscribeExceptionAsync");
        return Task.CompletedTask;
    }
}
