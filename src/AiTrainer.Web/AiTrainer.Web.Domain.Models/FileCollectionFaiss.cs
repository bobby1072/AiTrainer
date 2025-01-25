using System.Text.Json;
using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models
{
    public record FileCollectionFaiss : DomainModel<FileCollectionFaiss, long?>
    {
        [LockedData]
        public required Guid CollectionId { get; set; }
        public required byte[] FaissIndex { get; set; }
        public required JsonDocument FaissJson { get; set; }

        public override bool Equals(FileCollectionFaiss? other)
        {
            return other is FileCollectionFaiss fileCollectionFaiss
                && Id == fileCollectionFaiss.Id
                && CollectionId == fileCollectionFaiss.CollectionId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
