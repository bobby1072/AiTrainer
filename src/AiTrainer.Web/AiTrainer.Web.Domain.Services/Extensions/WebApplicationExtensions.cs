using AiTrainer.Web.Domain.Services.Hangfire.Abstract;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AiTrainer.Web.Domain.Services.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> UseHangfireAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var hangfireService = scope.ServiceProvider.GetRequiredService<IHangfireJobService>();
        hangfireService.RegisterJobs();

        if (app.Environment.IsDevelopment())
        {
            app.UseHangfireDashboard("/api/hangfire", new DashboardOptions
            {
                DarkModeEnabled = true
            });
        }
        
        return app;
    }
}