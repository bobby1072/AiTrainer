using System.ComponentModel.DataAnnotations.Schema;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("solicited_device_token", Schema = DbConstants.PublicSchema)]
    public record SolicitedDeviceTokenEntity : BaseEntity<Guid, SolicitedDeviceToken>
    {
        [Column("device_token")]
        public override Guid Id
        {
            get => base.Id;
            set => base.Id = value;
        }
        public required bool InUse { get; set; }
        public required DateTime SolicitedAt { get; set; }

        public override SolicitedDeviceToken ToModel()
        {
            return new SolicitedDeviceToken
            {
                InUse = InUse,
                SolicitedAt = SolicitedAt,
                Id = Id,
            };
        }
    }
}
