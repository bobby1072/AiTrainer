using System.ComponentModel.DataAnnotations;

namespace AiTrainer.Web.Persistence.Entities
{
    public abstract record BaseEntity<TId, TRuntime>
        where TRuntime : class
    {
        [Key]
        #nullable disable
        public TId Id { get; set; }
        #nullable enable
        public abstract TRuntime ToModel();
    }
}
