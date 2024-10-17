using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Client.Abstract;
using AiTrainer.Web.CoreClient.Client.Concrete;
using AiTrainer.Web.CoreClient.Models.Response;
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
                throw new InvalidDataException("Missing env vars");
            }

            serviceCollection.Configure<AiTrainerCoreConfiguration>(aiTrainerCoreSection);

            serviceCollection.AddHttpClient<
                ICoreClient<ChunkedDocument, string>,
                CoreClientChunkDocument
            >();

            return serviceCollection;
        }
    }
}
