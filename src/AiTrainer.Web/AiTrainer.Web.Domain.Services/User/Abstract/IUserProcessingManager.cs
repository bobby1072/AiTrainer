using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.User.Abstract
{
    public interface IUserProcessingManager : IDomainService
    {
        Task<Models.User> ConfirmUser(Models.User userToConfirm);
        Task<Models.SolicitedDeviceToken> IssueDeviceToken();
        Task<Models.User> FindAndCacheUser(Guid deviceToken);
        Task<Models.User?> TryGetUserFromCache(Guid deviceToken);
        Task<Models.User> UpdateUser(Models.User userToSave, Guid deviceToken);
    }
}
