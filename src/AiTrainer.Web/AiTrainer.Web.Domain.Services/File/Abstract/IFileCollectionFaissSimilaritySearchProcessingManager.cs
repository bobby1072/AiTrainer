using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.CoreClient.Models.Response;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

public interface IFileCollectionFaissSimilaritySearchProcessingManager
{
    Task<SimilaritySearchCoreResponse> SimilaritySearch(SimilaritySearchInput input, Domain.Models.User currentUser);
}