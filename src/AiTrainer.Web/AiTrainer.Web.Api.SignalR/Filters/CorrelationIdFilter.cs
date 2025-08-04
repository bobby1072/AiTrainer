using CommonApiConstants = BT.Common.Api.Helpers.ApiConstants;
using Microsoft.AspNetCore.SignalR;

namespace AiTrainer.Web.Api.SignalR.Filters;

internal class CorrelationIdFilter: IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var correlationIdForRequest = Guid.NewGuid().ToString();
        
        var httpContext = context.Context.GetHttpContext();
        httpContext?.Request.Headers.TryAdd(CommonApiConstants.CorrelationIdHeader, correlationIdForRequest);
        httpContext?.Response.Headers.TryAdd(CommonApiConstants.CorrelationIdHeader, correlationIdForRequest);

        
        return await next.Invoke(context);
    }
}