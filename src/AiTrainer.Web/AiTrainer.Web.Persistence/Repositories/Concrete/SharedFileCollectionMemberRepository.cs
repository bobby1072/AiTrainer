﻿using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using BT.Common.FastArray.Proto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete;

internal sealed class SharedFileCollectionMemberRepository: BaseRepository<SharedFileCollectionMemberEntity, Guid, SharedFileCollectionMember>
{
    public SharedFileCollectionMemberRepository(
        IDbContextFactory<AiTrainerContext> dbContextFactory,
        ILogger<SharedFileCollectionMemberRepository> logger
    )
        : base(dbContextFactory, logger) { }

    public override async Task<DbSaveResult<SharedFileCollectionMember>> Create(IReadOnlyCollection<SharedFileCollectionMember> entObj)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var documentEnts = entObj.FastArraySelect(RuntimeToEntity).ToArray();

            foreach (var mem in entObj)
            {
                await dbContext.SharedFileCollectionMembers.AddAsync(mem.ToEntity());
                
                await dbContext.SaveChangesAsync();
            }
            
            await UpdateFileColLastUpdate(dbContext.FileCollections,
                documentEnts.FastArraySelect(x => x.CollectionId)
                    .ToArray());
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new DbSaveResult<SharedFileCollectionMember>
            {
                Data = dbContext.SharedFileCollectionMembers.Local.FastArraySelect(x => x.ToModel()).ToArray(),
                IsSuccessful = true
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public override async Task<DbSaveResult<SharedFileCollectionMember>> Update(IReadOnlyCollection<SharedFileCollectionMember> entObj)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var documentEnts = entObj.FastArraySelect(RuntimeToEntity).ToArray();

            dbContext.SharedFileCollectionMembers.UpdateRange(documentEnts);
            
            await UpdateFileColLastUpdate(dbContext.FileCollections,
                documentEnts.FastArraySelect(x => x.CollectionId)
                    .ToArray());
            
            
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new DbSaveResult<SharedFileCollectionMember>
            {
                Data = dbContext.SharedFileCollectionMembers.Local.FastArraySelect(x => x.ToModel()).ToArray(),
                IsSuccessful = true
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public override async Task<DbDeleteResult<Guid>> Delete(IReadOnlyCollection<Guid> entIds)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.SharedFileCollectionMembers.Where(x => entIds.Contains(x.Id!)).ExecuteDeleteAsync();   
            
            await dbContext.SaveChangesAsync();
            
            await transaction.CommitAsync(); 
            
            return new DbDeleteResult<Guid>(entIds);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public override async Task<DbDeleteResult<SharedFileCollectionMember>> Delete(IReadOnlyCollection<SharedFileCollectionMember> entObj)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var documentEnts = entObj.FastArraySelect(RuntimeToEntity).ToArray();
            
            dbContext.SharedFileCollectionMembers.RemoveRange(documentEnts);
            
            await UpdateFileColLastUpdate(dbContext.FileCollections,
                documentEnts.FastArraySelect(x => x.CollectionId)
                    .ToArray());
            
            
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new DbDeleteResult<SharedFileCollectionMember>
            {
                Data = entObj,
                IsSuccessful = true
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    protected override SharedFileCollectionMemberEntity RuntimeToEntity(SharedFileCollectionMember runtimeObj)
    {
        return runtimeObj.ToEntity();
    }
}