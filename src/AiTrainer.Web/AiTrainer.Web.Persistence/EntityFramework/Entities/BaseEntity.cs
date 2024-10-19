namespace AiTrainer.Web.Persistence.EntityFramework.Entities
{
    public abstract record BaseEntity<TRuntime>
        where TRuntime : class
    {
        public abstract TRuntime ToModel();
    }
}
