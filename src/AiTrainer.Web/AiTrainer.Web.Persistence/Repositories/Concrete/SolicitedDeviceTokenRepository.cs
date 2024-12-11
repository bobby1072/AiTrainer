using System.Security.Cryptography;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class SolicitedDeviceTokenRepository
        : BaseRepository<SolicitedDeviceTokenEntity, Guid, SolicitedDeviceToken>,
            ISolicitedDeviceTokenRepository
    {
        private readonly ILogger<SolicitedDeviceTokenRepository> _logger;

        public SolicitedDeviceTokenRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<SolicitedDeviceTokenRepository> logger
        )
            : base(dbContextFactory, logger)
        {
            _logger = logger;
        }

        protected override SolicitedDeviceTokenEntity RuntimeToEntity(SolicitedDeviceToken runtime)
        {
            return runtime.ToEntity();
        }

        public async Task CleanUp()
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            async Task dbFunc()
            {
                await dbContext
                    .SolicitedDeviceTokens.Where(x =>
                        x.ExpiresAt < DateTime.UtcNow && x.InUse == false
                    )
                    .ExecuteDeleteAsync();

                await dbContext.SaveChangesAsync();
            }

            await TimeAndLogDbOperation(dbFunc, nameof(CleanUp), _entityType.Name);

            _logger.LogDebug("Cleaned up expired device tokens with in db");
        }
    }
}
