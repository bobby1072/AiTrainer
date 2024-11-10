namespace AiTrainer.Web.Domain.Models
{
    public class User : DomainModel<User, Guid?>
    {
        public required string Email { get; set; }

        public string? Name { get; set; }

        public required DateTime DateCreated { get; set; }

        public required DateTime DateModified { get; set; }

        public override bool Equals(DomainModel<User, Guid?>? other)
        {
            return other is User user
                && Id == user.Id
                && Email == user.Email
                && Name == user.Name
                && DateCreated == user.DateCreated
                && DateModified == user.DateModified;
        }

        public override void ApplyCreationDefaults()
        {
            DateCreated = DateTime.UtcNow;
            DateModified = DateTime.UtcNow;
        }

        public static string GetCacheKey(string accessToken) => $"{_cacheKey}{accessToken}";

        private const string _cacheKey = "cacheUser-";
    }
}
