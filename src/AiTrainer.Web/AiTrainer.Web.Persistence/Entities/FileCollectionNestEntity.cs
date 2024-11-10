using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities
{
    public record FileCollectionNestEntity : BaseEntity<long, FileCollectionNest>
    {
        public required Guid ParentCollectionId { get; set; }
        public FileCollectionEntity? ParentFileCollection { get; init; }
        public required Guid ChildCollectionId { get; set; }
        public FileCollectionEntity? ChildFileCollection { get; init; }

        public override FileCollectionNest ToModel() =>
            new FileCollectionNest
            {
                Id = Id,
                ParentCollectionId = ParentCollectionId,
                ParentFileCollection = ParentFileCollection?.ToModel(),
                ChildCollectionId = ChildCollectionId,
                ChildFileCollection = ChildFileCollection?.ToModel(),
            };
    }
}
