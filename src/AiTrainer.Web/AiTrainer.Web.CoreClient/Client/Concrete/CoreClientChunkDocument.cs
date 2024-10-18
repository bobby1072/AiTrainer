using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Client.Abstract;
using AiTrainer.Web.CoreClient.Exceptions;
using AiTrainer.Web.CoreClient.Models;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Client.Concrete
{
    internal class CoreClientChunkDocument
        : BaseCoreClient<string, ChunkedDocument>,
            ICoreClient<string, ChunkedDocument>
    {
        public CoreClientChunkDocument(
            HttpClient httpClient,
            ILogger<CoreClientChunkDocument> logger,
            IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig
        )
            : base(httpClient, logger, aiTrainerCoreConfig) { }

        public override async Task<ChunkedDocument> InvokeAsync(string documentTextToChunk)
        {
            if (string.IsNullOrEmpty(documentTextToChunk))
            {
                throw new CoreClientException(
                    CoreClientConstants.CoreClientExceptionConstants.NoDocumentToChunk
                );
            }
            var documentToChunk = new DocumentToChunk { DocumentText = documentTextToChunk };

            var data = await ExecuteRequest(
                CoreClientRequestType.Json,
                HttpMethod.Post,
                "chunkingrouter/chunk",
                nameof(CoreClientChunkDocument),
                documentTextToChunk
            );

            return data;
        }
    }
}
