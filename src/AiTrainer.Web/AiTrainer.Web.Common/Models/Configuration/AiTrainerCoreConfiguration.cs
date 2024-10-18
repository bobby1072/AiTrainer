namespace AiTrainer.Web.Common.Models.Configuration
{
    public class AiTrainerCoreConfiguration : BaseConfiguration
    {
        public static readonly string Key = GetKey(nameof(AiTrainerCoreConfiguration));
        public string ApiKey { get; init; }
        public string BaseEndpoint { get; init; }
    }
}
