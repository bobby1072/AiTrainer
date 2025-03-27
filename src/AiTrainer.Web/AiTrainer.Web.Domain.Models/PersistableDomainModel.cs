using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models
{
    public abstract class PersistableDomainModel<TEquatable, TId>: DomainModel<TEquatable> where TEquatable : DomainModel<TEquatable>
    {
        [LockedData]
        public TId Id { get; set; }
        public virtual void ApplyCreationDefaults() { }
    }
}
