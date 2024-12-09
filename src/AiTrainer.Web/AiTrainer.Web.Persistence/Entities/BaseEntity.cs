using System.ComponentModel.DataAnnotations;

namespace AiTrainer.Web.Persistence.Entities
{
    public abstract record BaseEntity<TId, TRuntime>
        where TRuntime : class
    {
        [Key]
        public virtual TId Id { get; set; }
        public abstract TRuntime ToModel();
    }
}
