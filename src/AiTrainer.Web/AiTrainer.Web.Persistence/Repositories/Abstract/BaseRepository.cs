using System.Reflection;
using System.Text;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using BT.Common.FastArray.Proto;
using BT.Common.OperationTimer.Proto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    internal abstract class BaseRepository<TEnt, TEntId, TModel> : IRepository<TEnt, TEntId, TModel>
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
    {
        protected readonly IDbContextFactory<AiTrainerContext> _contextFactory;
        private readonly ILogger<BaseRepository<TEnt, TEntId, TModel>> _logger;
        private static readonly Type _entityType = typeof(TEnt);
        private static readonly IReadOnlyCollection<PropertyInfo> _entityProperties =
            _entityType.GetProperties();

        protected BaseRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<BaseRepository<TEnt, TEntId, TModel>> logger
        )
        {
            _logger = logger;
            _contextFactory =
                dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        protected abstract TEnt RuntimeToEntity(TModel runtimeObj);

        public virtual async Task<DbResult<int>> GetCount()
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = dbContext.Set<TEnt>();
            var count = await TimeAndLogDbOperation(
                () => foundOneQuerySet.CountAsync(),
                nameof(GetCount),
                _entityType.Name
            );

            return new DbResult<int>(true, count);
        }

        public virtual async Task<DbGetManyResult<TModel>> GetMany<T>(
            T value,
            string propertyName,
            params string[] relations
        )
        {
            ThrowIfPropertyDoesNotExist<T>(propertyName);
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = AddRelationsToSet(dbContext.Set<TEnt>());
            var foundOne = await TimeAndLogDbOperation(
                () =>
                    foundOneQuerySet
                        .Where(x => EF.Property<T>(x, propertyName).Equals(value))
                        .ToArrayAsync(),
                nameof(GetMany),
                _entityType.Name
            );

            return new DbGetManyResult<TModel>(
                foundOne?.FastArraySelect(x => x.ToModel()).ToArray()
            );
        }

        public virtual async Task<DbGetManyResult<TModel>> GetMany(
            TEntId entityId,
            params string[] relations
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = AddRelationsToSet(dbContext.Set<TEnt>());
            var foundOne = await TimeAndLogDbOperation(
                () => foundOneQuerySet.Where(x => x.Id!.Equals(entityId)).ToArrayAsync(),
                nameof(GetMany),
                _entityType.Name
            );

            return new DbGetManyResult<TModel>(
                foundOne?.FastArraySelect(x => x.ToModel()).ToArray()
            );
        }

        public virtual async Task<DbGetOneResult<TModel>> GetOne(
            TEntId entityId,
            params string[] relations
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = AddRelationsToSet(dbContext.Set<TEnt>());
            var foundOne = await TimeAndLogDbOperation(
                () => foundOneQuerySet.FirstOrDefaultAsync(x => x.Id!.Equals(entityId)),
                nameof(GetOne),
                _entityType.Name
            );

            return new DbGetOneResult<TModel>(foundOne?.ToModel());
        }

        public virtual async Task<DbResult<bool>> Exists(TEntId entityId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = dbContext.Set<TEnt>();
            var foundOne = await TimeAndLogDbOperation(
                () => foundOneQuerySet.AnyAsync(x => x.Id!.Equals(entityId)),
                nameof(Exists),
                _entityType.Name
            );

            return new DbResult<bool>(true, foundOne);
        }

        public virtual async Task<DbResult<bool>> Exists<T>(
            T value,
            string propertyName,
            params string[] relations
        )
        {
            ThrowIfPropertyDoesNotExist<T>(propertyName);
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = AddRelationsToSet(dbContext.Set<TEnt>());
            var foundOne = await TimeAndLogDbOperation(
                () => foundOneQuerySet.AnyAsync(x => EF.Property<T>(x, propertyName).Equals(value)),
                nameof(Exists),
                _entityType.Name
            );

            return new DbResult<bool>(true, foundOne);
        }

        public virtual async Task<DbGetOneResult<TModel>> GetOne<T>(
            T value,
            string propertyName,
            params string[] relations
        )
        {
            ThrowIfPropertyDoesNotExist<T>(propertyName);
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = AddRelationsToSet(dbContext.Set<TEnt>());
            var foundOne = await TimeAndLogDbOperation(
                () =>
                    foundOneQuerySet.FirstOrDefaultAsync(x =>
                        EF.Property<T>(x, propertyName).Equals(value)
                    ),
                nameof(GetOne),
                _entityType.Name
            );

            return new DbGetOneResult<TModel>(foundOne?.ToModel());
        }

        public virtual async Task<DbSaveResult<TModel>> Create(IReadOnlyCollection<TModel> entObj)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            async Task<TModel?> operation()
            {
                await set.AddRangeAsync(entObj.FastArraySelect(x => RuntimeToEntity(x)));
                await dbContext.SaveChangesAsync();
                return null;
            }
            await TimeAndLogDbOperation(operation, nameof(Create), _entityType.Name);
            var runtimeObjs = set.Local.FastArraySelect(x => x.ToModel());
            return new DbSaveResult<TModel>(runtimeObjs.ToArray());
        }

        public virtual async Task<DbDeleteResult<TModel>> Delete(IReadOnlyCollection<TModel> entObj)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            async Task<TModel?> operation()
            {
                set.RemoveRange(entObj.FastArraySelect(x => RuntimeToEntity(x)));
                await dbContext.SaveChangesAsync();
                return null;
            }
            await TimeAndLogDbOperation(operation, nameof(Delete), _entityType.Name);
            return new DbDeleteResult<TModel>(entObj);
        }

        public virtual async Task<DbDeleteResult<TEntId>> Delete(IReadOnlyCollection<TEntId> entIds)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            async Task<TEntId?> operation()
            {
                await set.Where(x => entIds.Contains(x.Id!)).ExecuteDeleteAsync();
                await dbContext.SaveChangesAsync();
                return default;
            }
            await TimeAndLogDbOperation(operation, nameof(Delete), _entityType.Name);

            return new DbDeleteResult<TEntId>(entIds);
        }

        public virtual async Task<DbSaveResult<TModel>> Update(IReadOnlyCollection<TModel> entObj)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var set = dbContext.Set<TEnt>();
            async Task<TModel?> operation()
            {
                set.UpdateRange(entObj.FastArraySelect(x => RuntimeToEntity(x)));
                await dbContext.SaveChangesAsync();
                return null;
            }
            await TimeAndLogDbOperation(operation, nameof(Update), _entityType.Name);

            var runtimeObjs = set.Local.FastArraySelect(x => x.ToModel());
            return new DbSaveResult<TModel>(runtimeObjs.ToArray());
        }

        protected IQueryable<TEnt> AddRelationsToSet(
            IQueryable<TEnt> set,
            params string[] relations
        )
        {
            foreach (var relation in relations)
            {
                set = set.Include(relation);
            }

            return set;
        }

        protected async Task<T> TimeAndLogDbOperation<T>(
            Func<Task<T>> func,
            string operationName,
            string entityName,
            string? entityId = null
        )
        {
            var logMessageBuilder = new StringBuilder("performing {OperationName} on {EntityName}");
            if (entityId != null)
            {
                logMessageBuilder.Append(" with id {EntityId}");
            }
            _logger.LogDebug(logMessageBuilder.ToString(), operationName, entityName, entityId);

            var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(func);

            _logger.LogDebug(
                "finished {OperationName} on {EntityName} in {TimeTaken}ms",
                operationName,
                entityId is not null ? entityId : entityName,
                timeTaken.TotalMilliseconds
            );

            return result;
        }

        private static bool DoesPropertyExist<T>(string propertyName)
        {
            return _entityProperties.Any(x =>
                x.Name == propertyName && x.PropertyType == typeof(T)
            );
        }

        private static void ThrowIfPropertyDoesNotExist<T>(string propertyName)
        {
            if (!DoesPropertyExist<T>(propertyName))
            {
                throw new ArgumentException(
                    $"Property {propertyName} does not exist on entity {_entityType.Name}"
                );
            }
        }
    }
}
