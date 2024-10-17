using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Client.Abstract;
using AiTrainer.Web.CoreClient.Exceptions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Client.Concrete
{
    internal class CoreClientChunkDocument : BaseCoreClient, ICoreClientChunkDocument
    {
        public CoreClientChunkDocument(
            HttpClient httpClient,
            ILogger<CoreClientChunkDocument> logger,
            IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig
        )
            : base(httpClient, logger, aiTrainerCoreConfig) { }

        public async Task<ChunkedDocument> InvokeAsync(string? documentTextToChunk)
        {
            if (string.IsNullOrEmpty(documentTextToChunk))
            {
                throw new CoreClientException("No document text to chunk");
            }
            var documentToChunk = new DocumentToChunk { DocumentText = documentTextToChunk };

            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(
                    JsonSerializer.Serialize(documentToChunk),
                    Encoding.UTF8,
                    _applicationJson
                ),
                RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/chunk"),
            };

            AddApiKeyHeader(request);

            var data = await InvokeCoreRequest<ChunkedDocument>(
                request,
                nameof(CoreClientChunkDocument)
            );

            return data;
        }

        public async Task<ChunkedDocument?> TryInvokeAsync(string? documentTextToChunk)
        {
            try
            {
                return await InvokeAsync(documentTextToChunk);
            }
            catch (Exception coreClientException)
            {
                LogCoreError(coreClientException, nameof(CoreClientChunkDocument));
                return null;
            }
        }
    }
}
