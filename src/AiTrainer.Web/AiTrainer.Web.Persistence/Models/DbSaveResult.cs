namespace AiTrainer.Web.Persistence.Models
{
    public record DbSaveResult<TModel>: DbResult<IReadOnlyCollection<TModel>> where TModel : class
    {
        public DbSaveResult(IReadOnlyCollection<TModel>? models) : base(models?.Count > 0, models ?? Array.Empty<TModel>()) { }
    }
}
