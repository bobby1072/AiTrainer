namespace AiTrainer.Web.Domain.Models.Partials
{
    public record PartialFileDocument : DomainModelPartial<PartialFileDocument, Guid>
    {
        public required Guid CollectionId { get; set; }
        public required FileTypeEnum FileType { get; set; }
        public required string FileName { get; set; }
        public required DateTime DateCreated { get; set; }

        public override bool Equals(PartialFileDocument? obj)
        {
            return obj is PartialFileDocument fileDocument
                && Id == fileDocument.Id
                && CollectionId == fileDocument.CollectionId
                && DateCreated == fileDocument.DateCreated
                && FileType == fileDocument.FileType
                && FileName == fileDocument.FileName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
