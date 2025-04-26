using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("file_collection_faiss", Schema = DbConstants.PublicSchema)]
    public record FileCollectionFaissEntity : BaseEntity<long, FileCollectionFaiss>
    {
        public Guid? CollectionId { get; set; }
        public required byte[] FaissIndex { get; set; }
        public required JsonDocument FaissJson { get; set; }
        public required Guid UserId { get; set; }
        
        public required DateTime DateCreated { get; set; }
        public required DateTime DateModified { get; set; }
        public override FileCollectionFaiss ToModel() =>
            new()
            {
                Id = Id,
                UserId = UserId,
                CollectionId = CollectionId,
                FaissIndex = FaissIndex,
                FaissJson = FaissJson,
                DateCreated = DateCreated,
                DateModified = DateModified,
            };
    }
}
