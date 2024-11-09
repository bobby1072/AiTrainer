using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.User.Abstract
{
    public interface IUserProcessingManager: IDomainService
    {
        Task<Models.User> SaveUser(string accessToken);
    }
}