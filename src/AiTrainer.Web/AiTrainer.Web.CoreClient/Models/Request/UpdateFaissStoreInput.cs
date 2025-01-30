using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public record UpdateFaissStoreInput: BaseCoreClientRequestBody
{
    [JsonPropertyName("fileInput")]
    public required byte[] FileInput { get; init; }
    [JsonPropertyName("jsonDocStore")]
    public required JsonDocument JsonDocStore { get; init; }
    [JsonPropertyName("newDocuments")]
    public required CreateFaissStoreInput NewDocuments { get; init; }
}