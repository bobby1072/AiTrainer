using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("file_document", Schema = DbConstants.PublicSchema)]
    public record FileDocumentEntity : BaseEntity<Guid, FileDocument>
    {
        public required Guid CollectionId { get; set; }
        public DateTime DateCreated { get; set; }
        public required int FileType { get; set; }
        public required string FileName { get; set; }
        public required byte[] FileData { get; set; }
        public required byte[] FaissIndex { get; set; }
        public required JsonDocument FaissJson { get; set; }

        public override FileDocument ToModel() =>
            new FileDocument
            {
                Id = Id,
                CollectionId = CollectionId,
                DateCreated = DateCreated,
                FileType = (FileTypeEnum)FileType,
                FileData = FileData,
                FileName = FileName,
                FaissIndex = FaissIndex,
                FaissJson = FaissJson,
            };
    }
}