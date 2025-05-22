using AiTrainer.Web.Domain.Models.ApiModels.Request;
using BT.Common.FastArray.Proto;

namespace AiTrainer.Web.Domain.Models.Extensions;

public static class SharedFileCollectionMemberExtensions
{
    public static IReadOnlyCollection<SharedFileCollectionMember> ToSharedFileCollectionMember(this SharedFileCollectionMemberSaveInput sharedFileCollectionMember)
    {
        return sharedFileCollectionMember.MembersToShareTo.FastArraySelect(x => x.ToSharedFileCollectionMember(sharedFileCollectionMember.CollectionId)).ToArray();
    }


    public static bool CanAny(this IEnumerable<SharedFileCollectionMember> sharedFileCollectionMembers, Guid userId, Guid collectionId, SharedFileCollectionMemberPermission sharedFileCollectionMemberPermission)
    {
        return sharedFileCollectionMembers.Any(x => x.Can(userId, collectionId, sharedFileCollectionMemberPermission));
    }
    
    public static bool CanAll(this IEnumerable<SharedFileCollectionMember> sharedFileCollectionMembers, Guid userId, Guid collectionId, SharedFileCollectionMemberPermission sharedFileCollectionMemberPermission)
    {
        return sharedFileCollectionMembers.All(x => x.Can(userId, collectionId, sharedFileCollectionMemberPermission));
    }

    public static SharedFileCollectionMember ToSharedFileCollectionMember(
        this SharedFileCollectionSingleMemberSaveInput sharedFileCollectionSingleMemberSaveInput,
        Guid collectionId)
    {
        return new SharedFileCollectionMember
        {
            UserId = sharedFileCollectionSingleMemberSaveInput.UserId,
            CollectionId = collectionId,
            CanViewDocuments = sharedFileCollectionSingleMemberSaveInput.CanViewDocuments,
            CanCreateDocuments = sharedFileCollectionSingleMemberSaveInput.CanCreateDocuments,
            CanDownloadDocuments = sharedFileCollectionSingleMemberSaveInput.CanDownloadDocuments,
            CanRemoveDocuments = sharedFileCollectionSingleMemberSaveInput.CanRemoveDocuments,
        };
    }
}