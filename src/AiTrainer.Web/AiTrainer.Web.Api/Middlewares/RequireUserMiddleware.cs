using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class RequireUserMiddleware : BaseMiddleware
    {
        private readonly IDomainServiceActionExecutor _domainServiceExecutor;

        public RequireUserMiddleware(
            RequestDelegate next,
            IDomainServiceActionExecutor domainServiceExecutor
        )
            : base(next)
        {
            _domainServiceExecutor = domainServiceExecutor;
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            if (context.GetEndpoint()?.Metadata.GetMetadata<RequireUserAttribute>() is not null)
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
