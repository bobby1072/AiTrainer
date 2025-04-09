using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public record CoreRemoveDocumentsFromStoreInput
{
    [JsonIgnore] 
    public required byte[] FileInput { get; init; }
    public required JsonDocument JsonDocStore { get; init; }
    public required IReadOnlyCollection<Guid> DocumentIdsToRemove { get; init; }
}