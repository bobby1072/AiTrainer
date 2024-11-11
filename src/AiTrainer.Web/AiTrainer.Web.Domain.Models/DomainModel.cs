namespace AiTrainer.Web.Domain.Models
{
    public abstract record DomainModel<TEquatable, TId> : IEquatable<TEquatable>
        where TEquatable : DomainModel<TEquatable, TId>
    {
        public TId Id { get; set; }

        public abstract bool Equals(TEquatable? obj);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void ApplyCreationDefaults() { }
    }
}
