using System.Reflection;
using System.Text;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Persistence.EntityFramework.Contexts;
using AiTrainer.Web.Persistence.EntityFramework.Entities;
using BT.Common.FastArray.Proto;
using BT.Common.OperationTimer.Proto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.EntityFramework.Repositories.Abstract
{
    internal abstract class BaseRepository<TEnt, TModel> : IRepository<TEnt, TModel>
        where TEnt : BaseEntity<TModel>
        where TModel : class
    {
        protected readonly IDbContextFactory<AiTrainerContext> _contextFactory;
        protected abstract ILogger _logger { get; init; }
        private static readonly Type _entityType = typeof(TEnt);
        private static readonly Type _modelType = typeof(TModel);
        private static readonly IReadOnlyCollection<PropertyInfo> _entityProperties =
            _entityType.GetProperties();

        private static readonly IReadOnlyCollection<PropertyInfo> _modelProperties =
            _modelType.GetProperties();

        protected BaseRepository(IDbContextFactory<AiTrainerContext> dbContextFactory)
        {
            _contextFactory =
                dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public virtual async Task<int> GetCount()
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = dbContext.Set<TEnt>();
            return await TimeAndLogDbOperation(
                () => foundOneQuerySet.CountAsync(),
                nameof(GetCount),
                _entityType.Name
            );
        }

        public virtual async Task<IReadOnlyCollection<TModel>?> GetMany<T>(
            T value,
            string propertyName,
            params string[] relations
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = AddRelationsToSet(dbContext.Set<TEnt>());
            var foundOne = await TimeAndLogDbOperation(
                () =>
                    foundOneQuerySet
                        .Where(x => EF.Property<T>(x, propertyName.ToPascalCase()).Equals(value))
                        .ToArrayAsync(),
                nameof(GetMany),
                _entityType.Name
            );

            return foundOne?.FastArraySelect(x => x.ToModel()).ToArray();
        }

        public virtual async Task<TModel?> GetOne<T>(
            T value,
            string propertyName,
            params string[] relations
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundOneQuerySet = AddRelationsToSet(dbContext.Set<TEnt>());
            var foundOne = await TimeAndLogDbOperation(
                () =>
                    foundOneQuerySet.FirstOrDefaultAsync(x =>
                        EF.Property<T>(x, propertyName.ToPascalCase()).Equals(value)
                    ),
                nameof(GetOne),
                _entityType.Name
            );

            return foundOne?.ToModel();
        }

        public virtual async Task<IReadOnlyCollection<TModel>?> Create(
            IReadOnlyCollection<TModel> entObj
        )
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
            return runtimeObjs?.Count() > 0 ? runtimeObjs.OfType<TModel>().ToArray() : null;
        }

        public virtual async Task<IReadOnlyCollection<TModel>?> Delete(
            IReadOnlyCollection<TModel> entObj
        )
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
            return entObj;
        }

        public virtual async Task<IReadOnlyCollection<TModel>?> Update(
            IReadOnlyCollection<TModel> entObj
        )
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
            return runtimeObjs?.Count() > 0 ? runtimeObjs.OfType<TModel>().ToArray() : null;
        }

        protected IQueryable<TEnt> AddRelationsToSet(
            IQueryable<TEnt> set,
            params string[] relations
        )
        {
            ;
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
            _logger.LogInformation(
                logMessageBuilder.ToString(),
                operationName,
                entityName,
                entityId
            );

            var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(func);

            _logger.LogInformation(
                "finished {OperationName} on {EntityName} in {TimeTaken}ms",
                operationName,
                entityId is not null ? entityId : entityName,
                timeTaken.TotalMilliseconds
            );

            return result;
        }

        protected abstract TEnt RuntimeToEntity(TModel runtimeObj);
    }
}
