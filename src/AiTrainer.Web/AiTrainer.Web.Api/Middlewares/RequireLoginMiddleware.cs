using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class RequireLoginMiddleware : BaseMiddleware
    {
        private readonly ILogger<RequireLoginMiddleware> _logger;
        private readonly IDomainServiceActionExecutor _actionExecutor;
        public RequireLoginMiddleware(
            RequestDelegate requestDelegate,
            ILogger<RequireLoginMiddleware> logger,
            IDomainServiceActionExecutor actionExecutor
        )
            : base(requestDelegate)
        {
            _logger = logger;
            _actionExecutor = actionExecutor;
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if(endpoint?.Metadata.GetMetadata<RequireUserLoginAttribute>() is not null)
            {
                var deviceToken = context.GetDeviceToken();

                await _actionExecutor.ExecuteAsync<IUserProcessingManager, User>(service => service.FindAndCacheUser(deviceToken));
            }
            await _next.Invoke(context);
        }
    }
}