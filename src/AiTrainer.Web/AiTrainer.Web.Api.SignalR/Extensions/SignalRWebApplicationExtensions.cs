using AiTrainer.Web.Api.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;

namespace AiTrainer.Web.Api.SignalR.Extensions;

public static class SignalRWebApplicationExtensions
{
    public static WebApplication UseAiTrainerSignalR(this WebApplication app)
    {
        app
            .MapHub<AiTrainerHub>("Api/SignalR");
        
        return app;
    }
}