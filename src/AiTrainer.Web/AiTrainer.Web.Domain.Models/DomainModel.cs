using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models
{
    public abstract record DomainModel<TEquatable, TId> : IEquatable<TEquatable>
    {
        [JsonPropertyName("$type")]
        public string TypeName
        {
            get => GetType().Name;
        }
        public TId Id { get; set; }

        public abstract bool Equals(TEquatable? obj);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void ApplyCreationDefaults() { }
    }
}
