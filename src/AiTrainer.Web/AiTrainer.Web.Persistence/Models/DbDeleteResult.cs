namespace AiTrainer.Web.Persistence.Models
{
    public record DbDeleteResult<TModel> : DbResult<IReadOnlyCollection<TModel>>
    {
        public DbDeleteResult(IReadOnlyCollection<TModel>? models = null)
            : base(models?.Count > 0, models ?? Array.Empty<TModel>()) { }
    }
}
