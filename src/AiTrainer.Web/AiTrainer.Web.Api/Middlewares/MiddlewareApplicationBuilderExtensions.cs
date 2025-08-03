using BT.Common.Api.Helpers.Extensions;

namespace AiTrainer.Web.Api.Middlewares
{
    public static class MiddlewareApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAiTrainerDefaultMiddlewares(
            this IApplicationBuilder app
        )
        {
            app
                .UseMiddleware<ExceptionHandlingMiddleware>()
                .UseCorrelationIdMiddleware()
                .UseMiddleware<RequireLoginMiddleware>();

            return app;
        }
    }
}
