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
        public SolicitedDeviceTokenRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<SolicitedDeviceTokenRepository> logger
        )
            : base(dbContextFactory, logger) { }

        protected override SolicitedDeviceTokenEntity RuntimeToEntity(SolicitedDeviceToken runtime)
        {
            return runtime.ToEntity();
        }
    }
}
