namespace AiTrainer.Web.Persistence.EntityFramework.Entities
{
    internal abstract record BaseEntity<TRuntime>
        where TRuntime : class
    { }
}
