using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Response
{
    internal record CoreResponse<T>
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; init; }

        [JsonPropertyName("data")]
        public T? Data { get; init; }

        [JsonPropertyName("exceptionMessage")]
        public string? ExceptionMessage { get; init; }
    }
}
