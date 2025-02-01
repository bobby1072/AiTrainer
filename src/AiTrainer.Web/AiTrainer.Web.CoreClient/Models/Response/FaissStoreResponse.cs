using System.Text.Json;
 using System.Text.Json.Serialization;
 
 namespace AiTrainer.Web.CoreClient.Models.Response;
 
 public record FaissStoreResponse : BaseCoreClientResponseData
 {
     [JsonPropertyName("jsonDocStore")]
     public required JsonDocument JsonDocStore { get; init; }
     [JsonPropertyName("indexFile")]
     public required byte[] IndexFile { get; init; }
 }