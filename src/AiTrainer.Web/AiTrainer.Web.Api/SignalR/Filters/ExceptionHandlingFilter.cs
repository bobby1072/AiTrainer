using AiTrainer.Web.Common.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace AiTrainer.Web.Api.SignalR.Filters;

public class ExceptionHandlingFilter : IHubFilter
{
    private readonly ILogger<ExceptionHandlingFilter> _logger;
    public ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger)
    {
        _logger = logger;
    }
    public ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return next.Invoke(invocationContext);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Signal R connection: {ConnectionId} with correlationId {CorrelationId} failed with message {ExceptionMessage}",
                invocationContext.Context.ConnectionId,
                invocationContext.Context.GetHttpContext()?.GetCorrelationId(),
                e.Message);
            throw;
        }
    }
}