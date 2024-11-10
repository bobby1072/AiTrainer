using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class UserRepository : BaseRepository<UserEntity, Guid, User>
    {
        public UserRepository(IDbContextFactory<AiTrainerContext> dbContextFactory, ILogger<UserRepository> logger) : base(dbContextFactory, logger) { }

        protected override UserEntity RuntimeToEntity(User runtimeObj) => runtimeObj.ToEntity();
    }
}
