using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

public interface IFileCollectionFaissSimilaritySearchProcessingManager: IDomainService
{
    Task<SimilaritySearchCoreResponse> SimilaritySearch(SimilaritySearchInput input, Domain.Models.User currentUser);
}