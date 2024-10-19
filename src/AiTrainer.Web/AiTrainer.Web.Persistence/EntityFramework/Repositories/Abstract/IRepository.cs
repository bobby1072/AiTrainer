using AiTrainer.Web.Persistence.EntityFramework.Entities;

namespace AiTrainer.Web.Persistence.EntityFramework.Repositories.Abstract
{
    internal interface IRepository<TEnt, TModel>
        where TEnt : BaseEntity<TModel>
        where TModel : class
    {
        Task<int> GetCount();
        Task<TModel?> GetOne<T>(T value, string propertyName, params string[] relations);
        Task<IReadOnlyCollection<TModel>?> Create(IReadOnlyCollection<TModel> entObj);
        Task<IReadOnlyCollection<TModel>?> Delete(IReadOnlyCollection<TModel> entObj);
        Task<IReadOnlyCollection<TModel>?> Update(IReadOnlyCollection<TModel> entObj);
    }
}
