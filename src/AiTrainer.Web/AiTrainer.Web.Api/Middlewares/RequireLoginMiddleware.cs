using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.User.Abstract;
using System.Net;

namespace AiTrainer.Web.Api.Middlewares
{
    internal sealed class RequireLoginMiddleware
    {
        private readonly RequestDelegate _next;
        public RequireLoginMiddleware(
            RequestDelegate next
        )
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (
                context.GetEndpoint()?.Metadata.GetMetadata<RequireUserLoginAttribute>() is not null
            )
            {
                var accessToken = context.GetAccessToken();
                
                _ = await context.RequestServices
                        .GetRequiredService<IUserProcessingManager>()
                        .SaveAndCacheUser(accessToken)
                    ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);
            }
            await _next.Invoke(context);
        }
    }
}
