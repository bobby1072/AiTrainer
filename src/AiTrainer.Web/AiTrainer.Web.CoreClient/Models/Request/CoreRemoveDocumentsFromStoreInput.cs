using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public record CoreRemoveDocumentsFromStoreInput
{
    [JsonIgnore] public byte[] FileInput { get; init; } = [];
    public required JsonDocument DocStore { get; init; }
    public required IReadOnlyCollection<Guid> DocumentIdsToRemove { get; init; }
}