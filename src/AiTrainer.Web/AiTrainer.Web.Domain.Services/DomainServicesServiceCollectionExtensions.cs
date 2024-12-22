using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.File;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Domain.Services.Hangfire;
using AiTrainer.Web.Domain.Services.User;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Domain.Services.User.Concrete;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services
{
    public static class DomainServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            //var connectionString = configuration.GetConnectionString("DefaultConnection");

            //services.AddHangfire(configuration =>
            //        configuration
            //            ?.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            //            .UseSimpleAssemblyNameTypeSerializer()
            //            .UseRecommendedSerializerSettings()
            //            .UsePostgreSqlStorage(x => x.UseNpgsqlConnection(connectionString))
            //    )
            //    .AddHangfireServer(options =>
            //    {
            //        options.Queues = HangfireConstants.Queues.FullQueueList;
            //    });

            services
                .AddScoped<IUserProcessingManager, UserProcessingManager>()
                .AddScoped<IFileCollectionProcessingManager, FileCollectionProcessingManager>()
                .AddScoped<IFileDocumentProcessingManager, FileDocumentProcessingManager>()
                .AddScoped<IHealthService, HealthService>()
                .AddScoped<ICachingService, DistributedCachingService>()
                .AddTransient<IDomainServiceActionExecutor, DomainServiceActionExecutor>();

            return services;
        }
    }
}
