using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AiTrainer.Web.Common;
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

            serviceCollection.AddScoped<ICoreClient<CoreClientHealthResponse>>
                (sp => new CoreClientHealth(
                    GetLoggerForClient<CoreClientHealth>(sp),
                    GetConfigForClient(sp),
                    GetContextAccessorForClient(sp),
                    ApiConstants.DefaultCamelFlurlJsonSerializer
                ))
                .AddScoped<ICoreClient<CoreUpdateFaissStoreInput, CoreFaissStoreResponse>>
                (sp => new CoreClientUpdateFaissStore(
                    GetLoggerForClient<CoreClientUpdateFaissStore>(sp),
                    GetConfigForClient(sp),
                    GetContextAccessorForClient(sp),
                    ApiConstants.DefaultCamelFlurlJsonSerializer
                ))
                .AddScoped<ICoreClient<CoreSimilaritySearchInput, CoreSimilaritySearchResponse>>
                (sp => new CoreClientSimilaritySearch(
                    GetLoggerForClient<CoreClientSimilaritySearch>(sp),
                    GetConfigForClient(sp),
                    GetContextAccessorForClient(sp),
                    ApiConstants.DefaultCamelFlurlJsonSerializer
                ))
                .AddScoped<ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse>>
                (sp => new CoreClientFormattedChatQuery(
                    GetLoggerForClient<CoreClientFormattedChatQuery>(sp),
                    GetConfigForClient(sp),
                    GetContextAccessorForClient(sp),
                    ApiConstants.DefaultCamelFlurlJsonSerializer
                ));

            
            serviceCollection
                .AddHttpClient<ICoreClient<CoreCreateFaissStoreInput, CoreFaissStoreResponse>>();
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
        private static IOptionsSnapshot<AiTrainerCoreConfiguration> GetConfigForClient(IServiceProvider sp)
        {
            return sp.GetRequiredService<IOptionsSnapshot<AiTrainerCoreConfiguration>>();
        }
        private static IHttpContextAccessor GetContextAccessorForClient(IServiceProvider sp)
        {
            return sp.GetRequiredService<IHttpContextAccessor>();
        }
        private static ILogger<T> GetLoggerForClient<T>(IServiceProvider sp) where T :ICoreClient
        {
            return sp.GetRequiredService<ILoggerFactory>().CreateLogger<T>();
        }
    }
}
