using AiTrainer.Web.Domain.Models.ApiModels.Request;

namespace AiTrainer.Web.Domain.Models.Extensions;

public static class SharedFileCollectionMemberExtensions
{
    public static SharedFileCollectionMember ToSharedFileCollectionMember(this SharedFileCollectionMemberSaveInput sharedFileCollectionMember)
    {
        return new SharedFileCollectionMember
        {
            Id = sharedFileCollectionMember.Id,
            UserId = sharedFileCollectionMember.UserId,
            CollectionId = sharedFileCollectionMember.CollectionId,
            CanViewDocuments = sharedFileCollectionMember.CanViewDocuments,
            CanCreateDocuments = sharedFileCollectionMember.CanCreateDocuments,
            CanDownloadDocuments = sharedFileCollectionMember.CanDownloadDocuments,
            CanRemoveDocuments = sharedFileCollectionMember.CanRemoveDocuments,
        };
    }
}