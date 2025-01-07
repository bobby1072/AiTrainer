using AiTrainer.Web.Common;
using Microsoft.AspNetCore.SignalR;

namespace AiTrainer.Web.Api.SignalR.Filters;

public class CorrelationIdFilter: IHubFilter
{
    public ValueTask<object?> InvokeMethodAsync(HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var correlationIdForRequest = Guid.NewGuid().ToString();
        
        var httpContext = context.Context.GetHttpContext();
        httpContext?.Request.Headers.TryAdd(ApiConstants.CorrelationIdHeader, correlationIdForRequest);
        httpContext?.Response.Headers.TryAdd(ApiConstants.CorrelationIdHeader, correlationIdForRequest);

        
        return next.Invoke(context);
    }
}