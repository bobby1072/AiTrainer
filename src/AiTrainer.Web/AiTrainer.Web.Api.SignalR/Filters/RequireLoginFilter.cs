using System.Net;
using AiTrainer.Web.Api.SignalR.Extensions;
using AiTrainer.Web.Common.Attributes;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Microsoft.AspNetCore.SignalR;

namespace AiTrainer.Web.Api.SignalR.Filters;

internal class RequireLoginFilter: IHubFilter
{
    private readonly IUserProcessingManager _userProcessingManager;

    public RequireLoginFilter(IUserProcessingManager userProcessingManager)
    {
        _userProcessingManager = userProcessingManager;
    }
    
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        
        var hubRequireAttribute = context.GetMetadata<RequireUserLoginAttribute>();
        
        if (hubRequireAttribute is not null)
        {
            var hubHttpContext = context.Context.GetHttpContext();
            var userAccessToken = hubHttpContext.GetAccessToken();
                
            _ = await _userProcessingManager.SaveAndCacheUser(userAccessToken)
                ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);
        }
        
        
        return await next.Invoke(context);
    }
}