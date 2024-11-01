
namespace AiTrainer.Web.Persistence.Models
{
    public record DbGetManyResult<TModel>: DbResult<IReadOnlyCollection<TModel>> where TModel : class
    {
        public DbGetManyResult(IReadOnlyCollection<TModel>? models): base(models?.Count > 0, models ?? Array.Empty<TModel>()) { }
    }
}
