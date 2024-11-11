namespace AiTrainer.Web.Domain.Models.Partials
{
    public record PartialFileCollection : DomainModelPartial<PartialFileCollection, Guid>
    {
        public required Guid UserId { get; set; }
        public required string CollectionName { get; init; }
        public required DateTime DateCreated { get; init; }
        public required DateTime DateModified { get; init; }
        public Guid? ParentId { get; set; }

        public override bool Equals(PartialFileCollection? other)
        {
            return other is PartialFileCollection fileCollection
                && Id == fileCollection.Id
                && UserId == fileCollection.UserId
                && CollectionName == fileCollection.CollectionName
                && DateCreated == fileCollection.DateCreated
                && DateModified == fileCollection.DateModified;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
