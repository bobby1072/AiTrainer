using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.Abstract;
using Hangfire;

namespace AiTrainer.Web.Domain.Services.User.Abstract
{
    public interface IUserProcessingManager : IDomainService
    {
        Task<Models.User> ConfirmUser(SaveUserInput userToConfirm, Guid deviceToken);
        Task<Models.SolicitedDeviceToken> IssueDeviceToken();
        Task<Models.User> FindAndCacheUser(Guid deviceToken);
        Task<Models.User?> TryGetUserFromCache(Guid deviceToken);
        Task<Models.User> UpdateUser(SaveUserInput userToSave, Guid historicDeviceToken);

        [AutomaticRetry(
            Attempts = 2,
            LogEvents = true,
            DelaysInSeconds = [10],
            OnAttemptsExceeded = AttemptsExceededAction.Delete
        )]
        internal Task CleanUpDeviceTokens();
    }
}
