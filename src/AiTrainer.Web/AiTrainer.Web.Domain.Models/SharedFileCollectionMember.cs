namespace AiTrainer.Web.Domain.Models;

public class SharedFileCollectionMember : PersistableDomainModel<SharedFileCollectionMember, Guid?>
{
    public required Guid UserId { get; set; }
    public required Guid CollectionId { get; set; }
    public bool CanViewDocuments { get; set; }
    public bool CanDownloadDocuments { get; set; }
    public bool CanCreateDocuments { get; set; }
    public bool CanRemoveDocuments { get; set; }

    public override bool Equals(SharedFileCollectionMember? obj)
    {
        return obj != null
            && obj.Id == Id
            && obj.UserId == UserId
            && obj.CollectionId == CollectionId
            && obj.CanViewDocuments == CanViewDocuments
            && obj.CanDownloadDocuments == CanDownloadDocuments
            && obj.CanRemoveDocuments == CanRemoveDocuments
            && obj.CanCreateDocuments == CanCreateDocuments;
    }
}
