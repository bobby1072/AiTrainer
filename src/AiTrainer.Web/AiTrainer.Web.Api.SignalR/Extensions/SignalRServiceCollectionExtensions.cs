using AiTrainer.Web.Api.SignalR.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Api.SignalR.Extensions;

public static class SignalRServiceCollectionExtensions
{
    public static IServiceCollection AddAiTrainerSignalR(this IServiceCollection services)
    {
        services.AddSignalR(opts =>
        {
            opts.AddFilter<ExceptionHandlingFilter>();
            opts.AddFilter<CorrelationIdFilter>();
            opts.AddFilter<RequireLoginFilter>();
        });
        
        return services;
    }
}