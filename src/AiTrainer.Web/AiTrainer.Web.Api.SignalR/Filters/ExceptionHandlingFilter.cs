using AiTrainer.Web.Common.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Api.SignalR.Filters;

internal class ExceptionHandlingFilter : IHubFilter
{
    private readonly ILogger<ExceptionHandlingFilter> _logger;
    public ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger)
    {
        _logger = logger;
    }
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next.Invoke(invocationContext);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Signal R connection: {ConnectionId} with correlationId {CorrelationId} threw an exception with message {ExceptionMessage}",
                invocationContext.Context.ConnectionId,
                invocationContext.Context.GetHttpContext()?.GetCorrelationId(),
                e.Message);
            return null;
        }
    }
}