using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AiTrainer.Web.CoreClient.Extensions;
using BT.Common.HttpClient.Extensions;
using Flurl;
using Flurl.Http;

namespace AiTrainer.Web.CoreClient.Clients.Concrete
{
    internal class CoreClientChunkDocument : ICoreClient<DocumentToChunk, ChunkedDocument>
    {
        private readonly ILogger<CoreClientChunkDocument> _logger;
        private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
        public CoreClientChunkDocument(
            ILogger<CoreClientChunkDocument> logger,
            IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig
        )
        {
            _logger = logger;
            _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        }

        public async Task<ChunkedDocument?> TryInvokeAsync(DocumentToChunk param)
        {
            try
            {
                var response = await _aiTrainerCoreConfiguration.BaseEndpoint
                    .AppendPathSegment("api")
                    .AppendPathSegment("chunkingrouter")
                    .AppendPathSegment("chunkdocument")
                    .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
                    .PostJsonAsync(param)
                    .ReceiveJsonAsync<ChunkedDocument>(_aiTrainerCoreConfiguration);

                return response;
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(ex, "{NameOfOp} request failed with status code {StatusCode}",
                    nameof(CoreClientChunkDocument),
                    ex.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{NameOfOp} request failed",
                    nameof(CoreClientChunkDocument));
                return null;
            }
        }
    }
}
