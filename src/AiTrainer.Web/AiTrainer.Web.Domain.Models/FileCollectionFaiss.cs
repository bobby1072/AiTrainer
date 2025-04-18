using System.Text.Json;
using AiTrainer.Web.Domain.Models.Attributes;
using AiTrainer.Web.Domain.Models.Helpers;

namespace AiTrainer.Web.Domain.Models
{
    public class FileCollectionFaiss : PersistableDomainModel<FileCollectionFaiss, long?>
    {
        [LockedData]
        public required Guid? CollectionId { get; set; }
        public required byte[] FaissIndex { get; set; }
        public required JsonDocument FaissJson { get; set; }
        [LockedData]
        public required Guid UserId { get; set; }

        public Lazy<IReadOnlyCollection<SingleDocumentChunk>> SingleDocuments =>
            new(() => FaissHelper.GetDocumentChunksFromFaissDocStore(FaissJson));
        public override bool Equals(FileCollectionFaiss? other)
        {
            return Id == other?.Id
                && CollectionId == other?.CollectionId
                && UserId == other?.UserId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
