namespace AiTrainer.Web.Domain.Models;

public class SharedFileCollectionMember : PersistableDomainModel<SharedFileCollectionMember, Guid?>
{
    public required Guid UserId { get; set; }
    public required Guid CollectionId { get; set; }
    public Guid? ParentSharedMemberId { get; set; }
    public bool CanViewDocuments { get; set; }
    public bool CanDownloadDocuments { get; set; }
    public bool CanCreateDocuments { get; set; }
    public bool CanRemoveDocuments { get; set; }
    
    public bool Can(Guid userId, Guid collectionId, SharedFileCollectionMemberPermission sharedFileCollectionMemberPermission)
    {
        return UserId == userId && CollectionId == collectionId && Can(sharedFileCollectionMemberPermission);
    }
    
    
    
    public override bool Equals(SharedFileCollectionMember? obj)
    {
        return obj != null
            && obj.Id == Id
            && obj.UserId == UserId
            && obj.CollectionId == CollectionId
            && obj.CanViewDocuments == CanViewDocuments
            && obj.CanDownloadDocuments == CanDownloadDocuments
            && obj.CanRemoveDocuments == CanRemoveDocuments
            && obj.CanCreateDocuments == CanCreateDocuments
            && obj.ParentSharedMemberId == ParentSharedMemberId;
    }


    private bool Can(SharedFileCollectionMemberPermission sharedFileCollectionMemberPermission)
    {
        return sharedFileCollectionMemberPermission switch
        {
            SharedFileCollectionMemberPermission.ViewDocuments => CanViewDocuments,
            SharedFileCollectionMemberPermission.DownloadDocuments => CanDownloadDocuments,
            SharedFileCollectionMemberPermission.CreateDocuments => CanCreateDocuments,
            SharedFileCollectionMemberPermission.RemoveDocuments => CanRemoveDocuments,
            _ => throw new ArgumentOutOfRangeException(nameof(sharedFileCollectionMemberPermission), sharedFileCollectionMemberPermission, null)
        };
    }
}
