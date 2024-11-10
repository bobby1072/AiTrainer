using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class RequireLoginMiddleware : BaseMiddleware
    {
        private readonly IDomainServiceActionExecutor _domainServiceExecutor;

        public RequireLoginMiddleware(
            RequestDelegate next,
            IDomainServiceActionExecutor domainServiceExecutor
        )
            : base(next)
        {
            _domainServiceExecutor = domainServiceExecutor;
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            if (context.GetEndpoint()?.Metadata.GetMetadata<RequireLoginAttribute>() is not null)
            {
                var accessToken = context.GetAccessToken();
                await _domainServiceExecutor.ExecuteAsync<IUserProcessingManager, User>(
                    (userService) => userService.SaveAndCacheUser(accessToken)
                );
            }
            await _next.Invoke(context);
        }
    }
}
