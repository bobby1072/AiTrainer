using AiTrainer.Web.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using BT.Common.FastArray.Proto;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("file_document", Schema = DbConstants.PublicSchema)]
    public sealed record FileDocumentEntity : BaseEntity<Guid, FileDocument>
    {
        public Guid? CollectionId { get; set; }
        public required DateTime DateCreated { get; set; }
        public required Guid UserId { get; set; }
        public required int FileType { get; set; }
        public required string FileName { get; set; }
        public required byte[] FileData { get; set; }
        public string? FileDescription { get; set; }
        public bool FaissSynced { get; set; }
        public FileDocumentMetaDataEntity? MetaData { get; init; }

        public override FileDocument ToModel() =>
            new()
            {
                Id = Id,
                CollectionId = CollectionId,
                DateCreated = DateCreated,
                FileType = (FileTypeEnum)FileType,
                FileData = FileData,
                UserId = UserId,
                FaissSynced = FaissSynced,
                FileName = FileName,
                FileDescription = FileDescription,
                MetaData = MetaData?.ToModel(),
            };
    }
}
