using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Views;
using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Abstract
{
    public interface IFileCollectionProcessingManager : IDomainProcessingManager
    {
        Task<FileCollection> SaveFileCollectionAsync(FileCollectionSaveInput fileCollectionInput, Domain.Models.User currentUser);
        Task<Guid> DeleteFileCollectionAsync(Guid collectionId, Domain.Models.User currentUser);
        Task<FlatFileDocumentPartialCollectionView> GetOneLayerFileDocPartialsAndCollectionsAsync(
            Domain.Models.User currentUser, Guid? collectionId = null
        );
        Task<FileCollection> GetFileCollectionWithContentsAsync(Guid fileCollectionId, Domain.Models.User currentUser);

        Task<IReadOnlyCollection<SharedFileCollectionMember>> ShareFileCollectionAsync(
            SharedFileCollectionMemberSaveInput sharedFileColInput, Domain.Models.User currentUser);
    }
}
