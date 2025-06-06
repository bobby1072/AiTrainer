using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.User.Abstract
{
    public interface IUserProcessingManager : IDomainProcessingManager
    {
        Task<Domain.Models.User> SaveAndCacheUser(string accessToken);
        Task<Domain.Models.User?> TryGetUserFromCache(string accessToken);
    }
}
