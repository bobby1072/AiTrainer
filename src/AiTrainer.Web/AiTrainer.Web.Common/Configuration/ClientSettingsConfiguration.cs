namespace AiTrainer.Web.Common.Configuration
{
    public class ClientSettingsConfiguration : BaseConfiguration
    {
        public static readonly string Key = GetKey(nameof(ClientSettingsConfiguration));
        public required string Scope { get; init; }
        public required string InternalUserInfoEndpoint { get; init; }
        public required string AuthorityHost { get; init; }
        public required string InternalAuthorityHost { get; init; }
        public required string AuthorityClientId { get; init; }
    }
}
