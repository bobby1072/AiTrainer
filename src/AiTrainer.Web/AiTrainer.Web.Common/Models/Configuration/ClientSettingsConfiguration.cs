namespace AiTrainer.Web.Common.Models.Configuration
{
    public class ClientSettingsConfiguration: BaseConfiguration
    {
        public static readonly string Key = GetKey(nameof(ClientSettingsConfiguration));
        public required string Scope { get; init; }
        public required string UserInfoEndpoint { get; init; }
        public required string AuthorityHost { get; init; }
        public required string AuthorityClientId { get; init; }
    }
}
