using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models.ApiModels.Request;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

public interface IFileCollectionFaissSimilaritySearchProcessingManager: IFileCollectionFaissProcessingManager
{
    Task<CoreSimilaritySearchResponse> SimilaritySearch(SimilaritySearchInput input, Domain.Models.User currentUser);
}