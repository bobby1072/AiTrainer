using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Abstract
{
    public interface IFileCollectionProcessingManager : IDomainService
    {
        Task<FileCollection> SaveFileCollection(FileCollectionSaveInput fileCollectionInput, Domain.Models.User currentUser);
        Task<Guid> DeleteFileCollection(Guid collectionId, Domain.Models.User currentUser);
        Task<FlatFileDocumentPartialCollection> GetOneLayerFileDocPartialsAndCollections(
            Domain.Models.User currentUser, Guid? collectionId = null
        );
        Task<FileCollection> GetFileCollectionWithContents(Guid fileCollectionId, Domain.Models.User currentUser);
    }
}
