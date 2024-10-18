using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Client.Abstract;
using AiTrainer.Web.CoreClient.Client.Concrete;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.CoreClient.Service;
using AiTrainer.Web.CoreClient.Service.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.CoreClient
{
    public static class CoreClientServiceExtensions
    {
        public static IServiceCollection AddCoreClient(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
        )
        {
            var aiTrainerCoreSection = configuration.GetSection(AiTrainerCoreConfiguration.Key);

            if (!aiTrainerCoreSection.Exists())
            {
                throw new InvalidDataException(ExceptionConstants.MissingEnvVars);
            }

            serviceCollection.Configure<AiTrainerCoreConfiguration>(aiTrainerCoreSection);

            serviceCollection.AddHttpClient<
                ICoreClient<string, ChunkedDocument>,
                CoreClientChunkDocument
            >();

            serviceCollection.AddScoped<ICoreClientServiceProvider, CoreClientServiceProvider>();

            return serviceCollection;
        }
    }
}
