using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Response
{
    internal record CoreResponse
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; init; }
        [JsonPropertyName("exceptionMessage")]
        public string? ExceptionMessage { get; init; }
    }
    internal record CoreResponse<T>: CoreResponse
    {
        [JsonPropertyName("data")]
        public T? Data { get; init; }
    }
}
