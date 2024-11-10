namespace AiTrainer.Web.Domain.Models
{
    public record FileCollectionNest : DomainModel<FileCollectionNest, long?>
    {
        public required Guid ParentCollectionId { get; set; }
        public FileCollection? ParentFileCollection { get; init; }
        public required Guid ChildCollectionId { get; set; }
        public FileCollection? ChildFileCollection { get; init; }

        public override bool Equals(FileCollectionNest? other)
        {
            return Id == other?.Id
                && ParentCollectionId == other?.ParentCollectionId
                && ChildCollectionId == other.ChildCollectionId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
