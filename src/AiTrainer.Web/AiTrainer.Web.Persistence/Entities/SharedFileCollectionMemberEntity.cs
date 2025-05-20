using System.ComponentModel.DataAnnotations.Schema;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities;

[Table("shared_file_collection_member", Schema = DbConstants.PublicSchema)]
public sealed record SharedFileCollectionMemberEntity : BaseEntity<Guid, SharedFileCollectionMember>
{
    public required Guid UserId { get; set; }
    public required Guid CollectionId { get; set; }
    public bool CanViewDocuments { get; set; }
    public bool CanDownloadDocuments { get; set; }
    public bool CanCreateDocuments { get; set; }
    public bool CanRemoveDocuments { get; set; }

    public override SharedFileCollectionMember ToModel()
    {
        return new SharedFileCollectionMember
        {
            Id = Id,
            UserId = UserId,
            CollectionId = CollectionId,
            CanViewDocuments = CanViewDocuments,
            CanDownloadDocuments = CanDownloadDocuments,
            CanCreateDocuments = CanCreateDocuments,
            CanRemoveDocuments = CanRemoveDocuments,
        };
    }
}
