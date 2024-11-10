using AiTrainer.Web.Domain.Models;
using BT.Common.FastArray.Proto;

namespace AiTrainer.Web.Persistence.Entities
{
    public record FileCollectionEntity : BaseEntity<Guid, FileCollection>
    {
        public required Guid UserId { get; set; }

        public required string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public virtual IReadOnlyCollection<FileDocumentEntity>? FileDocuments { get; init; }
        public virtual IReadOnlyCollection<FileCollectionNestEntity>? CollectionNests { get; init; }

        public override FileCollection ToModel() =>
            new FileCollection
            {
                Id = Id,
                UserId = UserId,
                Name = Name,
                DateCreated = DateCreated,
                DateModified = DateModified,
                CollectionNests = CollectionNests?.FastArraySelect(x => x.ToModel()).ToList(),
                FileDocuments = FileDocuments?.FastArraySelect(x => x.ToModel()).ToList(),
            };
    }
}
