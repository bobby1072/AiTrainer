using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace AiTrainer.Web.CoreClient.Clients.Concrete
{
    internal class CoreClientChunkDocument : BaseCoreClient<DocumentToChunk, ChunkedDocument>
    {
        private const string _endpoint = "api/chunkingrouter/chunkdocument";
        public CoreClientChunkDocument(
            HttpClient httpClient,
            ILogger<CoreClientChunkDocument> logger,
            IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig
        )
            : base(httpClient, aiTrainerCoreConfig, logger) { }

        protected override HttpRequestMessage BuildMessage(DocumentToChunk param)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = JsonContent.Create(param),
                RequestUri = _aiTrainerCoreConfiguration.BaseEndpoint.AppendPathToUrl(
                    _endpoint
                ),
            };
            AddApiKeyHeader(request);
            
            return request;
        }
    }
}
