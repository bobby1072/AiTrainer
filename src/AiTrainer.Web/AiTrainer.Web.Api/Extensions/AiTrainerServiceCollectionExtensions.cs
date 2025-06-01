using AiTrainer.Web.Api.SignalR.Extensions;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.Extensions;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.UserInfoClient;

namespace AiTrainer.Web.Api.Extensions;

internal static class AiTrainerServiceCollectionExtensions
{
    public static IServiceCollection AddAiTrainerServices(this IServiceCollection services, IConfiguration config, IWebHostEnvironment hostEnvironment)
    {
        services
            .AddCoreClient(config)
            .AddSqlPersistence(config, hostEnvironment.IsDevelopment())
            .AddUserInfoClient(config)
            .AddAiTrainerSignalR()
            .AddDomainModelServices()
            .AddDomainServices(config);

        return services;
    }
}