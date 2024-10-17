using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiTrainer.Web.CoreClient.Client.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;

namespace AiTrainer.Web.CoreClient.Client.Concrete
{
    public partial class CoreClient : ICoreClient
    {
        public async Task<IReadOnlyCollection<string>> ChunkDocument(string documentTextToChunk)
        {
            var documentToChunk = new DocumentToChunk { DocumentText = documentTextToChunk };

            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(
                    JsonSerializer.Serialize(documentToChunk),
                    Encoding.UTF8,
                    "application/json"
                ),
                RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/chunk"),
            };

            AddApiKeyHeader(request);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCodeAndThrowCoreClientException();

            var data = await response.Content.ReadFromJsonAsync<CoreResponse<ChunkedDocument>>();

            var actualData = data.EnsureSuccessfulResponseAndGetData();

            return actualData.DocumentChunks;
        }

        public async Task<IReadOnlyCollection<string>?> TryChunkDocument(string documentTextToChunk)
        {
            try
            {
                return await ChunkDocument(documentTextToChunk);
            }
            catch (Exception coreClientException)
            {
                LogCoreError(coreClientException, nameof(ChunkDocument));
                return null;
            }
        }
    }
}
