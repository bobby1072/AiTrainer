using System.Text.Json;

namespace AiTrainer.Web.CoreClient.Models.Response;

public record FaissStoreResponse : BaseCoreClientResponseData
 {
     public required JsonDocument JsonDocStore { get; init; }
     public required byte[] IndexFile { get; init; }
 }