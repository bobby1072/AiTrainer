using AiTrainer.Web.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("file_document", Schema = DbConstants.PublicSchema)]
    public record FileDocumentEntity : BaseEntity<Guid, FileDocument>
    {
        public required Guid CollectionId { get; set; }
        public DateTime DateCreated { get; set; }
        public required int FileType { get; set; }

        public override FileDocument ToModel() =>
            new FileDocument
            {
                Id = Id,
                CollectionId = CollectionId,
                DateCreated = DateCreated,
                FileType = (FileTypeEnum)FileType,
            };
    }
}
