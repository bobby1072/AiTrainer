namespace AiTrainer.Web.Persistence.Models
{
    public record DbDeleteResult<TModel>: DbResult<IReadOnlyCollection<TModel>> where TModel : class
    {
        public DbDeleteResult(IReadOnlyCollection<TModel>? models) : base(models?.Count > 0, models ?? Array.Empty<TModel>()) { }
    }
}
