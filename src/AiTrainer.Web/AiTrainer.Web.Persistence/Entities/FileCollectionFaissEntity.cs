using System.ComponentModel.DataAnnotations.Schema;
using AiTrainer.Web.Domain.Models;
using System.Text.Json;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("file_collection_faiss", Schema = DbConstants.PublicSchema)]
    public record FileCollectionFaissEntity : BaseEntity<long, FileCollectionFaiss>
    {
        public required Guid? CollectionId { get; set; }
        public required byte[] FaissIndex { get; set; }
        public required JsonDocument FaissJson { get; set; }

        public override FileCollectionFaiss ToModel() =>
            new()
            {
                Id = Id,
                CollectionId = CollectionId,
                FaissIndex = FaissIndex,
                FaissJson = FaissJson,
            };
    }
}
