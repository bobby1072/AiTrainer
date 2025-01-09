namespace AiTrainer.Web.Persistence.Models
{
    public record DbGetManyResult<TModel> : DbResult<IReadOnlyCollection<TModel>>
        where TModel : class
    {
        public DbGetManyResult(IReadOnlyCollection<TModel>? models = null)
            : base(models?.Count > 0, models ?? []) { }
    }
}
