namespace AiTrainer.Web.Common.Models.Configuration
{
    public class AiTrainerCoreConfiguration : BaseConfiguration
    {
        public static readonly string Key = GetKey(nameof(AiTrainerCoreConfiguration));
        public required string ApiKey { get; init; }
        public required string BaseEndpoint { get; init; }
        public required int TimeoutInSeconds { get; init; }
        public required int TotalAttempts { get; init; }
        public required int DelayBetweenAttemptsInSeconds { get; init; }
    }
}
