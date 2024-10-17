using System.Net.Http.Json;
using AiTrainer.Web.CoreClient.Models.Response;

namespace AiTrainer.Web.CoreClient.Extensions
{
    internal static class HttpClientExtensions
    {
        public static async Task<T> InvokeCoreRequest<T>(
            this HttpClient httpClient,
            HttpRequestMessage request
        )
        {
            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCodeAndThrowCoreClientException();

            var data = await response.Content.ReadFromJsonAsync<CoreResponse<T>>();

            var actualData = data.EnsureSuccessfulResponseAndGetData();

            return actualData;
        }
    }
}
