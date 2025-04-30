using BT.Common.Polly.Models.Abstract;

namespace AiTrainer.Web.Common.Configuration
{
    public class AiTrainerCoreConfiguration : BaseConfiguration, IPollyRetrySettings
    {
        public static readonly string Key = GetKey(nameof(AiTrainerCoreConfiguration));
        public required int DocumentChunkingType { get; init; }
        public required string ApiKey { get; init; }
        public required string BaseEndpoint { get; init; }
        public int? TimeoutInSeconds { get; init; }
        public int? TotalAttempts { get; init; }
        public int? DelayBetweenAttemptsInSeconds { get; init; }
        public bool? UseJitter => false;
    }
}
