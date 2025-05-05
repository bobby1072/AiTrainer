
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BT.Common.Helpers.Extensions;

namespace AiTrainer.Web.CoreClient.Models.Request
{
    public sealed record CoreDocumentToChunkInput
    {
        public required IReadOnlyCollection<CoreSingleDocumentToChunk> DocumentsToChunk { get; init; }

        [JsonIgnore]
        internal CoreDocumentToChunkInputChunkingTypeEnum CoreDocumentToChunkInputChunkingType { get; set; } =
            CoreDocumentToChunkInputChunkingTypeEnum.Recursive;
        public string ChunkingType => CoreDocumentToChunkInputChunkingType.GetDisplayName();
    }
    public sealed record CoreSingleDocumentToChunk
    {
        public required string DocumentText { get; init; }
        public required Guid FileDocumentId { get; init; }
        public Dictionary<string, string> Metadata { get; init; } = [];
    }
    internal enum CoreDocumentToChunkInputChunkingTypeEnum
    {
        [Display(Name = "semantic")]
        Semantic = 1,
        [Display(Name = "recursive")]
        Recursive = 2
    }
}
