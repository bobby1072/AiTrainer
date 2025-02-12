using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Abstract
{
    public interface IFileCollectionProcessingManager : IDomainService
    {
        Task<FileCollection> SaveFileCollection(FileCollectionSaveInput fileCollectionInput, Models.User currentUser);
        Task<Guid> DeleteFileCollection(Guid collectionId, Models.User currentUser);
        Task<FlatFileDocumentPartialCollection> GetOneLayerFileDocPartialsAndCollections(
            Models.User currentUser, Guid? collectionId = null
        );
        Task<FileCollection> GetFileCollectionWithContents(Guid fileCollectionId, Models.User currentUser);
    }
}
