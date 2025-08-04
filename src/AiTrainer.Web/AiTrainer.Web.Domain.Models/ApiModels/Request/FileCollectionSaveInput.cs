using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request
{
    public sealed record FileCollectionSaveInput
    {
        public Guid? Id { get; init; }
        public Guid? ParentId { get; init; }
        public required string CollectionName { get; init; }
        public bool? AutoFaissSync { get; init; }
        public string? CollectionDescription { get; set; }
        public DateTime? DateCreated { get; init; }
        public DateTime? DateModified { get; init; }
    }
}
