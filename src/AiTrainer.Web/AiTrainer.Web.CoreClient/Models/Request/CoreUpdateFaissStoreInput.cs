using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public record CoreUpdateFaissStoreInput
{
    [JsonIgnore] 
    public byte[] FileInput { get; init; } = [];
    public required JsonDocument DocStore { get; init; }
    public required CoreCreateFaissStoreInput NewDocuments { get; init; }
}