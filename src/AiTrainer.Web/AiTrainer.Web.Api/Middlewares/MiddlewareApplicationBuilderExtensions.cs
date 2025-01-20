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
                .UseMiddleware<CorrelationIdMiddleware>()
                .UseMiddleware<RequireLoginMiddleware>();

            return app;
        }
    }
}
