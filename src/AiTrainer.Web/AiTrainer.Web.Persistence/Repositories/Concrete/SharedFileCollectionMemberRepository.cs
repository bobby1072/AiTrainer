using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete;

internal class SharedFileCollectionMemberRepository: BaseRepository<SharedFileCollectionMemberEntity, Guid, SharedFileCollectionMember>
{
    public SharedFileCollectionMemberRepository(
        IDbContextFactory<AiTrainerContext> dbContextFactory,
        ILogger<SharedFileCollectionMemberRepository> logger
    )
        : base(dbContextFactory, logger) { }


    protected override SharedFileCollectionMemberEntity RuntimeToEntity(SharedFileCollectionMember runtimeObj)
    {
        return runtimeObj.ToEntity();
    }
}