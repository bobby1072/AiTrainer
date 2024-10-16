namespace AiTrainer.Web.Persistence.Entities
{
    internal abstract record BaseEntity<TRuntime>
        where TRuntime : class { }
}
