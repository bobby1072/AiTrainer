namespace AiTrainer.Web.Common.Models.Configuration
{
    public class AiTrainerCoreConfiguration : BaseConfiguration
    {
        public static readonly string Key = GetKey(nameof(AiTrainerCoreConfiguration));
        public required string ApiKey { get; init; }
        public required string BaseEndpoint { get; init; }
    }
}
