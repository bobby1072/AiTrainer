using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    public interface IRepository<TEnt, TEntId, TModel>
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
    {
        Task<DbResult<int>> GetCount();
        Task<DbGetManyResult<TModel>> GetMany<T>(
            T value,
            string propertyName,
            params string[] relations
        );
        Task<DbGetManyResult<TModel>> GetMany(TEntId entityId, params string[] relations);
        Task<DbGetManyResult<TModel>> GetMany(params TEntId[] entityIds);
        Task<DbGetOneResult<TModel>> GetOne(TEntId entityId, params string[] relations);
        Task<DbGetOneResult<TModel>> GetOne<T>(
            T value,
            string propertyName,
            params string[] relations
        );
        Task<DbSaveResult<TModel>> Create(IReadOnlyCollection<TModel> entObj);
        Task<DbSaveResult<TModel>> Update(IReadOnlyCollection<TModel> entObj);
        Task<DbDeleteResult<TModel>> Delete(IReadOnlyCollection<TModel> entObj);
        Task<DbDeleteResult<TEntId>> Delete(IReadOnlyCollection<TEntId> entIds);
        Task<DbResult<bool>> Exists<T>(T value, string propertyName, params string[] relations);
        Task<DbResult<bool>> Exists(TEntId entityId);
    }
}
