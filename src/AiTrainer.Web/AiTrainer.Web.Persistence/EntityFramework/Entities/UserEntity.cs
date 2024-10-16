using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("user", Schema = DbConstants.PublicSchema)]
    internal record UserEntity : BaseEntity<UserEntity>
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public string Username { get; set; }
    }
}
