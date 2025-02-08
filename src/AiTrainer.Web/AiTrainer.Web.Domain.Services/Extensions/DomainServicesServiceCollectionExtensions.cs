using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Domain.Services.User.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services.Extensions
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
            var faissSyncedConfig = configuration.GetSection(FaissSyncRetrySettingsConfiguration.Key);

            if (!faissSyncedConfig.Exists())
            {
                throw new Exception("FaissSyncRetrySettingsConfiguration not found in configuration");
            }
            
            services.Configure<FaissSyncRetrySettingsConfiguration>(faissSyncedConfig);
            
            services
                .AddScoped<IUserProcessingManager, UserProcessingManager>()
                .AddScoped<IFileCollectionProcessingManager, FileCollectionProcessingManager>()
                .AddScoped<IFileDocumentProcessingManager, FileDocumentProcessingManager>()
                .AddScoped<IFileCollectionFaissSyncProcessingManager, FileCollectionFaissSyncProcessingManager>()
                .AddScoped<IHealthService, HealthService>()
                .AddScoped<ICachingService, DistributedCachingService>()
                .AddTransient<IDomainServiceActionExecutor, DomainServiceActionExecutor>();

            return services;
        }
    }
}
