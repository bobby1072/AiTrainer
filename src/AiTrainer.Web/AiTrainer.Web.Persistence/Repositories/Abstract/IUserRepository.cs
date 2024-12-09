using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    public interface IUserRepository : IRepository<UserEntity, Guid, User>
    {
        Task ConfirmAndBuildUserTransaction(User user);
    }
}
