namespace AiTrainer.Web.Common.Models.Configuration
{
    public class ClientSettingsConfiguration: BaseConfiguration
    {
        public static readonly string Key = GetKey(nameof(ClientSettingsConfiguration));
        public string Scope { get; init; }
        public string UserInfoEndpoint { get; init; }
        public string AuthorityHost { get; init; }
        public string AuthorityClientId { get; init; }
    }
}
