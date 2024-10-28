using System.ComponentModel.DataAnnotations;

namespace AiTrainer.Web.Persistence.EntityFramework.Entities
{
    public abstract record BaseEntity<TId,TRuntime>
        where TRuntime : class
    {
        [Key]
        public TId Id { get; set; }
        public abstract TRuntime ToModel();
    }
}
