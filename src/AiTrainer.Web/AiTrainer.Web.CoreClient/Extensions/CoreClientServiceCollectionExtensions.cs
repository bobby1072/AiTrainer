using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Clients.Concrete;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.Http.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using AiTrainer.Web.Common;

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
                    ICoreClient<DocumentToChunkInput, ChunkedDocumentResponse>
                >(sp => new CoreClientChunkDocument(
                         GetLoggerForClient<CoreClientChunkDocument>(sp),
                         GetConfigForClient(sp),
                         GetContextAccessorForClient(sp),
                         ApiConstants.DefaultCamelFlurlJsonSerializer
                ))
                .AddScoped<ICoreClient<CoreClientHealthResponse>>
                (sp => new CoreClientHealth(
                         GetLoggerForClient<CoreClientHealth>(sp),
                         GetConfigForClient(sp),
                         GetContextAccessorForClient(sp),
                         ApiConstants.DefaultCamelFlurlJsonSerializer
                ))
                .AddScoped<ICoreClient<CreateFaissStoreInput, FaissStoreResponse>>
                (sp => new CoreClientCreateFaissStore(
                         GetLoggerForClient<CoreClientCreateFaissStore>(sp),
                         GetConfigForClient(sp),
                         GetContextAccessorForClient(sp),
                         ApiConstants.DefaultCamelFlurlJsonSerializer
                ))
                .AddScoped<ICoreClient<UpdateFaissStoreInput, FaissStoreResponse>>
                (sp => new CoreClientUpdateFaissStore(
                         GetLoggerForClient<CoreClientUpdateFaissStore>(sp),
                         GetConfigForClient(sp),
                         GetContextAccessorForClient(sp),
                         ApiConstants.DefaultCamelFlurlJsonSerializer
                ))
                .AddScoped<ICoreClient<CoreSimilaritySearchInput, SimilaritySearchCoreResponse>>
                (sp => new CoreClientSimilaritySearch(
                         GetLoggerForClient<CoreClientSimilaritySearch>(sp),
                         GetConfigForClient(sp),
                         GetContextAccessorForClient(sp),
                         ApiConstants.DefaultCamelFlurlJsonSerializer
                ));

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
