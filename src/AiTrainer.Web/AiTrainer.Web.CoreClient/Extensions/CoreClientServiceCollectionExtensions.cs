using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.CoreClient.Extensions
{
    public static class CoreClientServiceCollectionExtensions
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

            serviceCollection
                .AddScoped<
                    ICoreClient<DocumentToChunkInput, ChunkedDocumentResponse>,
                    CoreClientChunkDocument
                >()
                .AddScoped<ICoreClient<CoreClientHealthResponse>, CoreClientHealth>()
                .AddScoped<ICoreClient<CreateFaissStoreInput, FaissStoreResponse>, CoreClientCreateFaissStore>()
                .AddScoped<ICoreClient<UpdateFaissStoreInput, FaissStoreResponse>, CoreClientUpdateFaissStore>()
                .AddScoped<ICoreClient<SimilaritySearchInput, SimilaritySearchResponse>, CoreClientSimilaritySearch>();

            return serviceCollection;
        }
    }
}
