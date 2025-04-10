using AiTrainer.Web.Common.Configuration;
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
        public static IServiceCollection AddDomainServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var faissSyncedConfig = configuration.GetSection(
                FaissSyncRetrySettingsConfiguration.Key
            );

            if (!faissSyncedConfig.Exists())
            {
                throw new Exception(
                    "FaissSyncRetrySettingsConfiguration not found in configuration"
                );
            }

            services.Configure<FaissSyncRetrySettingsConfiguration>(faissSyncedConfig);

            services
                .AddScoped<IUserProcessingManager, UserProcessingManager>()
                .AddScoped<IFileCollectionProcessingManager, FileCollectionProcessingManager>()
                .AddScoped<IFileDocumentProcessingManager, FileDocumentProcessingManager>()
                .AddScoped<
                    IFileCollectionFaissSimilaritySearchProcessingManager,
                    FileCollectionFaissSimilaritySearchProcessingManager
                >()
                .AddScoped<IFileCollectionFaissRemoveDocumentsProcessingManager, FileCollectionFaissRemoveDocumentsProcessingManager>()
                .AddScoped<
                    IFileCollectionFaissSyncProcessingManager,
                    FileCollectionFaissSyncProcessingManager
                >()
                .AddSingleton<
                    IFileCollectionFaissSyncBackgroundJobQueue,
                    FileCollectionFaissBackgroundJobQueue
                >()
                .AddHostedService<FileCollectionFaissBackgroundJobService>()
                .AddScoped<IHealthService, HealthService>()
                .AddScoped<ICachingService, DistributedCachingService>()
                .AddTransient<IDomainServiceActionExecutor, DomainServiceActionExecutor>();

            return services;
        }
    }
}
