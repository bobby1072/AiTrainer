using BT.Common.Polly.Models.Abstract;

namespace AiTrainer.Web.Common.Models.Configuration
{
    public class AiTrainerCoreConfiguration : BaseConfiguration, IPollyRetrySettings
    {
        public static readonly string Key = GetKey(nameof(AiTrainerCoreConfiguration));
        public required string ApiKey { get; init; }
        public required string BaseEndpoint { get; init; }
        public int? TimeoutInSeconds { get; init; }
        public int? TotalAttempts { get; init; }
        public int? DelayBetweenAttemptsInSeconds { get; init; }
        public bool? UseJitter => false;
    }
}
