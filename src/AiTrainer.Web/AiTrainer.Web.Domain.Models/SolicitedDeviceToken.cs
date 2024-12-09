using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models
{
    public record SolicitedDeviceToken : DomainModel<SolicitedDeviceToken, Guid?>
    {
        public required bool InUse { get; set; }

        [LockedProperty]
        public required DateTime SolicitedAt { get; set; }

        public override bool Equals(SolicitedDeviceToken? obj)
        {
            return obj?.InUse == InUse && SolicitedAt == obj.SolicitedAt && obj.Id == Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
