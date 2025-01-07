using AiTrainer.Web.Common.Attributes;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.User.Abstract;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class RequireLoginMiddleware : BaseMiddleware
    {
        public RequireLoginMiddleware(
            RequestDelegate next
        )
            : base(next)
        {
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            if (
                context.GetEndpoint()?.Metadata.GetMetadata<RequireUserLoginAttribute>() is not null
            )
            {
                var accessToken = context.GetAccessToken();
                await context.RequestServices.GetRequiredService<IUserProcessingManager>().SaveAndCacheUser(accessToken);
            }
            await _next.Invoke(context);
        }
    }
}
