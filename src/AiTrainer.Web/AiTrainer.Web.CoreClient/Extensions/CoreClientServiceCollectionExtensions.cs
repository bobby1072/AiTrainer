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

namespace AiTrainer.Web.CoreClient.Extensions
{
    public static class CoreClientServiceCollectionExtensions
    {
        private static readonly JsonSerializerOptions _serializerOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        private static readonly DefaultFlurlJsonSerializer _defaultSeralizer = new(_serializerOpts);
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
                         sp.GetLoggerForClient<CoreClientChunkDocument>(),
                         sp.GetConfigForClient(),
                         sp.GetContextAccessorForClient(),
                         _defaultSeralizer
                ))
                .AddScoped<ICoreClient<CoreClientHealthResponse>>
                (sp => new CoreClientHealth(
                         sp.GetLoggerForClient<CoreClientHealth>(),
                         sp.GetConfigForClient(),
                         sp.GetContextAccessorForClient(),
                         _defaultSeralizer
                ))
                .AddScoped<ICoreClient<CreateFaissStoreInput, FaissStoreResponse>>
                (sp => new CoreClientCreateFaissStore(
                         sp.GetLoggerForClient<CoreClientCreateFaissStore>(),
                         sp.GetConfigForClient(),
                         sp.GetContextAccessorForClient(),
                         _defaultSeralizer
                ))
                .AddScoped<ICoreClient<UpdateFaissStoreInput, FaissStoreResponse>>
                (sp => new CoreClientUpdateFaissStore(
                         sp.GetLoggerForClient<CoreClientUpdateFaissStore>(),
                         sp.GetConfigForClient(),
                         sp.GetContextAccessorForClient(),
                         _defaultSeralizer
                ))
                .AddScoped<ICoreClient<CoreSimilaritySearchInput, SimilaritySearchCoreResponse>>
                (sp => new CoreClientSimilaritySearch(
                         sp.GetLoggerForClient<CoreClientSimilaritySearch>(),
                         sp.GetConfigForClient(),
                         sp.GetContextAccessorForClient(),
                         _defaultSeralizer
                ));

            return serviceCollection;
        }
        public static IOptionsSnapshot<AiTrainerCoreConfiguration> GetConfigForClient(this IServiceProvider sp)
        {
            return sp.GetRequiredService<IOptionsSnapshot<AiTrainerCoreConfiguration>>();
        }
        public static IHttpContextAccessor GetContextAccessorForClient(this IServiceProvider sp)
        {
            return sp.GetRequiredService<IHttpContextAccessor>();
        }
        public static ILogger<T> GetLoggerForClient<T>(this IServiceProvider sp) where T :ICoreClient
        {
            return sp.GetRequiredService<ILoggerFactory>().CreateLogger<T>();
        }
    }
}
