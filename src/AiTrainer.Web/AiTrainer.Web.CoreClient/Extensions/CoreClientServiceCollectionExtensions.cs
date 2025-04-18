using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Domain.Models;

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
                .AddHttpClient<ICoreClient<CoreUpdateFaissStoreInput, CoreFaissStoreResponse>, CoreClientUpdateFaissStore>();
            serviceCollection
                .AddHttpClient<ICoreClient<CoreSimilaritySearchInput, CoreSimilaritySearchResponse>, CoreClientSimilaritySearch>();
            serviceCollection
                .AddHttpClient<ICoreClient<CoreClientHealthResponse>, CoreClientHealth>();
            serviceCollection
                .AddHttpClient<ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse>, CoreClientFormattedChatQuery>();
            serviceCollection
                .AddHttpClient<ICoreClient<CoreCreateFaissStoreInput, CoreFaissStoreResponse>, CoreClientCreateFaissStore>();
            serviceCollection
                .AddHttpClient<
                    ICoreClient<CoreDocumentToChunkInput, CoreChunkedDocumentResponse>,
                    CoreClientChunkDocument
                >();
            serviceCollection
                .AddHttpClient<ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse>,
                    CoreClientRemoveDocumentsFromStore>();
            
            return serviceCollection;
        }
    }
}
