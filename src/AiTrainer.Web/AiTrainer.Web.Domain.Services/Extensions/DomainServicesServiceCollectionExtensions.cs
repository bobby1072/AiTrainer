using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.File;
using AiTrainer.Web.Domain.Services.Hangfire;
using AiTrainer.Web.Domain.Services.Hangfire.Abstract;
using AiTrainer.Web.Domain.Services.Hangfire.Concrete;
using AiTrainer.Web.Domain.Services.User;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Http;
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
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services
                .AddHangfire(config =>
                    config
                        ?.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UsePostgreSqlStorage(x => x.UseNpgsqlConnection(connectionString))
                )
                .AddHangfireServer(options =>
                {
                    options.Queues = HangfireConstants.Queues.FullQueueList;
                });

            services
                .AddUserServices()
                .AddFileServices()
                .AddTransient<IHangfireJobService, HangfireJobService>()
                .AddTransient<IApiRequestHttpContextService>(serviceProvider =>
                {
                    var foundContextAccessor =
                        serviceProvider.GetRequiredService<IHttpContextAccessor>();
                    return new ApiRequestHttpContextService(foundContextAccessor.HttpContext);
                })
                .AddTransient<IDomainServiceActionExecutor, DomainServiceActionExecutor>()
                .AddScoped<ICachingService, DistributedCachingService>();

            return services;
        }
    }
}
