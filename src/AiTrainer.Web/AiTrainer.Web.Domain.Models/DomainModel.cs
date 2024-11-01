using System.Reflection;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models
{
    public abstract class DomainModel<TId>
    {
        [JsonPropertyName("id")]
        public TId? Id { get; set; }
        public abstract bool Equals(DomainModel<TId>? other);

        public new virtual int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void ApplyDefaults() { }
}
