using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Client.Abstract;
using AiTrainer.Web.CoreClient.Models;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Client.Concrete
{
    internal class CoreClientChunkDocument
        : BaseCoreClient<DocumentToChunk, ChunkedDocument>,
            ICoreClient<DocumentToChunk, ChunkedDocument>
    {
        protected override string _endpoint => "chunkingrouter/chunkdocument";
        protected override CoreClientRequestType _requestType => CoreClientRequestType.Json;
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
