using AiTrainer.Web.Persistence.EntityFramework.Entities;
using AiTrainer.Web.Persistence.Models;

namespace AiTrainer.Web.Persistence.EntityFramework.Repositories.Abstract
{
    public interface IRepository<TEnt,TEntId, TModel>
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
    {
        Task<DbResult<int>> GetCount();
        Task<DbGetOneResult<TModel>> GetOne<T>(T value, string propertyName, params string[] relations);
        Task<DbGetManyResult<TModel>> GetMany<T>(
            T value,
            string propertyName,
            params string[] relations
        );
        Task<DbSaveResult<TModel>> Create(IReadOnlyCollection<TModel> entObj);
        Task<DbSaveResult<TModel>> Update(IReadOnlyCollection<TModel> entObj);
        Task<DbDeleteResult<TModel>> Delete(IReadOnlyCollection<TModel> entObj);
    }
}
