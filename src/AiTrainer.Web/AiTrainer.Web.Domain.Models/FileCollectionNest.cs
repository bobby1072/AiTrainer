namespace AiTrainer.Web.Domain.Models
{
    public class FileCollectionNest : DomainModel<FileCollectionNest, long?>
    {
        public required Guid ParentCollectionId { get; set; }
        public FileCollection? ParentFileCollection { get; init; }
        public required Guid ChildCollectionId { get; set; }
        public FileCollection? ChildFileCollection { get; init; }

        public override bool Equals(DomainModel<FileCollectionNest, long?>? other)
        {
            return other is FileCollectionNest fileCollectionNest
                && Id == fileCollectionNest.Id
                && ParentCollectionId == fileCollectionNest.ParentCollectionId
                && ChildCollectionId == fileCollectionNest.ChildCollectionId;
        }
    }
}
