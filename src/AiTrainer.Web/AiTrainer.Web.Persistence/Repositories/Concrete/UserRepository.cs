using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class UserRepository : BaseRepository<UserEntity, Guid, User>, IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<UserRepository> logger
        )
            : base(dbContextFactory, logger)
        {
            _logger = logger;
        }

        protected override UserEntity RuntimeToEntity(User runtimeObj) => runtimeObj.ToEntity();

        public async Task ConfirmAndBuildUserTransaction(User user)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var userEntity = RuntimeToEntity(user);

                var solicitedTokenOperationJOb = dbContext
                    .SolicitedDeviceTokens.Where(x => x.Id == user.Id)
                    .ExecuteUpdateAsync(x => x.SetProperty(y => y.InUse, true));

                var addAsyncJob = dbContext.Users.AddAsync(userEntity);

                await Task.WhenAll(solicitedTokenOperationJOb, addAsyncJob.AsTask());

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Error occurred while trying to confirm and build user transaction with message {EMessage}",
                    e.Message
                );
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
