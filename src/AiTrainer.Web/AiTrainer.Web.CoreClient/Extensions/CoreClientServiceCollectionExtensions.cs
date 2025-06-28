using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using BT.Common.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            var aiTrainerCoreConfig = aiTrainerCoreSection
                .Get<AiTrainerCoreConfiguration>() ?? throw new InvalidDataException(ExceptionConstants.MissingEnvVars);

            serviceCollection.Configure<AiTrainerCoreConfiguration>(aiTrainerCoreSection);

            serviceCollection.AddHttpClientWithResilience<
                ICoreClient<CoreUpdateFaissStoreInput, CoreFaissStoreResponse>,
                CoreClientUpdateFaissStore
            >(aiTrainerCoreConfig);
            serviceCollection.AddHttpClientWithResilience<
                ICoreClient<CoreSimilaritySearchInput, CoreSimilaritySearchResponse>,
                CoreClientSimilaritySearch
            >(aiTrainerCoreConfig);
            serviceCollection.AddHttpClientWithResilience<
                ICoreClient<CoreClientHealthResponse>,
                CoreClientHealth
            >(aiTrainerCoreConfig);
            serviceCollection.AddHttpClientWithResilience<
                ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse>,
                CoreClientFormattedChatQuery
            >(aiTrainerCoreConfig);
            serviceCollection.AddHttpClientWithResilience<
                ICoreClient<CoreCreateFaissStoreInput, CoreFaissStoreResponse>,
                CoreClientCreateFaissStore
            >(aiTrainerCoreConfig);
            serviceCollection.AddHttpClientWithResilience<
                ICoreClient<CoreDocumentToChunkInput, CoreChunkedDocumentResponse>,
                CoreClientChunkDocument
            >(aiTrainerCoreConfig);
            serviceCollection.AddHttpClientWithResilience<
                ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse>,
                CoreClientRemoveDocumentsFromStore
            >(aiTrainerCoreConfig);

            return serviceCollection;
        }
    }
}
