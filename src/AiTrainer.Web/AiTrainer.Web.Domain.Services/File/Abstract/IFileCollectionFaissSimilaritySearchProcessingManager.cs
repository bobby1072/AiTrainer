using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

public interface IFileCollectionFaissSimilaritySearchProcessingManager: IFileCollectionFaissProcessingManager
{
    Task<IReadOnlyCollection<SingleDocumentChunk>> SimilaritySearch(SimilaritySearchInput input, Domain.Models.User currentUser, CancellationToken token = default);
}