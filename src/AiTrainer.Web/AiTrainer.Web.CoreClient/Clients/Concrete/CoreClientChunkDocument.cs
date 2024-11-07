using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mime;

namespace AiTrainer.Web.CoreClient.Clients.Concrete
{
    internal class CoreClientChunkDocument
        : BaseCoreClient<DocumentToChunk, ChunkedDocument>
    {
        protected override string _endpoint => "chunkingrouter/chunkdocument";
        protected override CoreClientRequestType _requestType => CoreClientRequestType.ApplicationJson;
        protected override HttpMethod _httpMethod => HttpMethod.Post;
        protected override ILogger _logger { get; init; }

        public CoreClientChunkDocument(
            HttpClient httpClient,
            ILogger<CoreClientChunkDocument> logger,
            IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig
        )
            : base(httpClient, aiTrainerCoreConfig)
        {
            _logger = logger;
        }
    }
}
