using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class RequireLoginMiddleware : BaseMiddleware
    {
        private readonly IUserProcessingManager _userProcessingManager;
        public RequireLoginMiddleware(
            RequestDelegate next,
            IUserProcessingManager userProcessingManager
        )
            : base(next)
        {
            _userProcessingManager = userProcessingManager;
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            if (
                context.GetEndpoint()?.Metadata.GetMetadata<RequireUserLoginAttribute>() is not null
            )
            {
                var accessToken = context.GetAccessToken();
                await _userProcessingManager.SaveAndCacheUser(accessToken);
            }
            await _next.Invoke(context);
        }
    }
}
