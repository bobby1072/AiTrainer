namespace AiTrainer.Web.Domain.Models
{
    public class FileDocument : DomainModel<FileDocument, Guid?>
    {
        public required Guid CollectionId { get; set; }
        public required DateTime DateCreated { get; set; }
        public required FileTypeEnum FileType { get; set; }

        public override bool Equals(DomainModel<FileDocument, Guid?>? other)
        {
            return other is FileDocument fileDocument
                && Id == fileDocument.Id
                && CollectionId == fileDocument.CollectionId
                && DateCreated == fileDocument.DateCreated;
        }
    }
}
