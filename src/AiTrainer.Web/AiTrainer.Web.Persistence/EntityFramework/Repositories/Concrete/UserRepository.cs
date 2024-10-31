using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.EntityFramework.Contexts;
using AiTrainer.Web.Persistence.EntityFramework.Entities;
using AiTrainer.Web.Persistence.EntityFramework.Repositories.Abstract;
using AiTrainer.Web.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.EntityFramework.Repositories.Concrete
{
    internal class UserRepository: BaseRepository<UserEntity, Guid, User>
    {
        public UserRepository(IDbContextFactory<AiTrainerContext> dbContextFactory, ILogger<UserRepository> logger): base(dbContextFactory, logger) { }

        protected override UserEntity RuntimeToEntity(User runtimeObj) => runtimeObj.ToEntity();
    }
}
