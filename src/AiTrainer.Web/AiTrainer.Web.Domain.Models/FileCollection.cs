namespace AiTrainer.Web.Domain.Models
{
    public class FileCollection : DomainModel<FileCollection, Guid?>
    {
        public required Guid UserId { get; set; }

        public required string Name { get; set; }

        public required DateTime DateCreated { get; set; }

        public required DateTime DateModified { get; set; }

        public IReadOnlyCollection<FileCollectionNest>? CollectionNests { get; set; }

        public IReadOnlyCollection<FileDocument>? FileDocuments { get; set; }

        public override bool Equals(DomainModel<FileCollection, Guid?>? other)
        {
            return other is FileCollection fileCollection
                && Id == fileCollection.Id
                && UserId == fileCollection.UserId
                && Name == fileCollection.Name
                && DateCreated == fileCollection.DateCreated
                && DateModified == fileCollection.DateModified;
        }

        public override void ApplyCreationDefaults()
        {
            DateModified = DateTime.UtcNow;
            DateCreated = DateTime.UtcNow;
        }
    }
}
