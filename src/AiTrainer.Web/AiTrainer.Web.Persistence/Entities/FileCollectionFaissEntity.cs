using AiTrainer.Web.Domain.Models;
using System.Text.Json;

namespace AiTrainer.Web.Persistence.Entities
{
    public record FileCollectionFaissEntity : BaseEntity<long, FileCollectionFaiss>
    {
        public required Guid CollectionId { get; set; }
        public required byte[] FaissIndex { get; set; }
        public required JsonDocument FaissJson { get; set; }

        public override FileCollectionFaiss ToModel() =>
            new FileCollectionFaiss
            {
                Id = Id,
                CollectionId = CollectionId,
                FaissIndex = FaissIndex,
                FaissJson = FaissJson
            };
    }
}