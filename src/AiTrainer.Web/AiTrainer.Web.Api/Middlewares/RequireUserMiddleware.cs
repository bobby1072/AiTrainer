
using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.User.Abstract;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class RequireUserMiddleware : BaseMiddleware
    {
        private readonly IUserProcessingManager _userProcessingManager;
        private readonly ILogger<RequireUserMiddleware> _logger;
        public RequireUserMiddleware(RequestDelegate next, IUserProcessingManager userProcessingManager, ILogger<RequireUserMiddleware> logger) : base(next)
        {
            _userProcessingManager = userProcessingManager;
            _logger = logger;
        }

        public override Task InvokeAsync(HttpContext context)
        {
            if (context.GetEndpoint()?.Metadata.GetMetadata<RequireUserAttribute>() is not null)
            {
                var correlationId = context.GetCorrelationId();


            }
            return _next.Invoke(context);
        }
    }
}