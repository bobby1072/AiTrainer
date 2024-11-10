namespace AiTrainer.Web.Domain.Models
{
    public record FileCollection : DomainModel<FileCollection, Guid?>
    {
        public required Guid UserId { get; set; }

        public required string Name { get; set; }

        public required DateTime DateCreated { get; set; }

        public required DateTime DateModified { get; set; }

        public IReadOnlyCollection<FileCollectionNest>? CollectionNests { get; init; }

        public IReadOnlyCollection<FileDocument>? FileDocuments { get; init; }

        public override bool Equals(FileCollection? other)
        {
            return Id == other?.Id
                && UserId == other?.UserId
                && Name == other.Name
                && DateCreated == other.DateCreated
                && DateModified == other.DateModified;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void ApplyCreationDefaults()
        {
            DateModified = DateTime.UtcNow;
            DateCreated = DateTime.UtcNow;
        }
    }
}
