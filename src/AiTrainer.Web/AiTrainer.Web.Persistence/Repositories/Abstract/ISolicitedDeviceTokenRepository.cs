using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    public interface ISolicitedDeviceTokenRepository
        : IRepository<SolicitedDeviceTokenEntity, Guid, SolicitedDeviceToken> { }
}
