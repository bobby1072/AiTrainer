using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
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

            serviceCollection.Configure<AiTrainerCoreConfiguration>(aiTrainerCoreSection);

            serviceCollection.AddHttpClient<
                ICoreClient<CoreUpdateFaissStoreInput, CoreFaissStoreResponse>,
                CoreClientUpdateFaissStore
            >(ConfigureClientTimeout);
            serviceCollection.AddHttpClient<
                ICoreClient<CoreSimilaritySearchInput, CoreSimilaritySearchResponse>,
                CoreClientSimilaritySearch
            >(ConfigureClientTimeout);
            serviceCollection.AddHttpClient<
                ICoreClient<CoreClientHealthResponse>,
                CoreClientHealth
            >(ConfigureClientTimeout);
            serviceCollection.AddHttpClient<
                ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse>,
                CoreClientFormattedChatQuery
            >(ConfigureClientTimeout);
            serviceCollection.AddHttpClient<
                ICoreClient<CoreCreateFaissStoreInput, CoreFaissStoreResponse>,
                CoreClientCreateFaissStore
            >(ConfigureClientTimeout);
            serviceCollection.AddHttpClient<
                ICoreClient<CoreDocumentToChunkInput, CoreChunkedDocumentResponse>,
                CoreClientChunkDocument
            >(ConfigureClientTimeout);
            serviceCollection.AddHttpClient<
                ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse>,
                CoreClientRemoveDocumentsFromStore
            >(ConfigureClientTimeout);

            return serviceCollection;
        }

        private static void ConfigureClientTimeout(
            IServiceProvider serviceProvider,
            HttpClient httpClient
        )
        {
            var coreClientOptsSingleton = serviceProvider.GetRequiredService<
                IOptions<AiTrainerCoreConfiguration>
            >();
            
            if (coreClientOptsSingleton.Value.TimeoutInSeconds is int timeout)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeout + 1);
            }
        }
    }
}
