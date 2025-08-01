using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models
{
    public class User : PersistableDomainModel<User, Guid?>
    {
        [SensitiveProperty]
        public required string Email { get; set; }
        public string? Name { get; set; }

        [LockedData]
        public required DateTime DateCreated { get; set; }
        public required DateTime DateModified { get; set; }
        public GlobalFileCollectionConfig? GlobalFileCollectionConfig { get; init; }
        public override bool Equals(User? other)
        {
            return other is User user
                && Id == user.Id
                && Email == user.Email
                && Name == user.Name
                && DateCreated == user.DateCreated
                && DateModified == user.DateModified;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void ApplyCreationDefaults()
        {
            DateCreated = DateTime.UtcNow;
            DateModified = DateTime.UtcNow;
        }
    }
}
