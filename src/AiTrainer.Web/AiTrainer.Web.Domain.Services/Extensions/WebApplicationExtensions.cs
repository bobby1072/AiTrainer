using Microsoft.AspNetCore.Builder;

namespace AiTrainer.Web.Domain.Services;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var hangfireService = scope.ServiceProvider.GetRequiredService<IHangfireJobsService>();
            hangfireService.RegisterJobs();
        }
    }
}