using BT.Common.Polly.Models.Abstract;

namespace AiTrainer.Web.Common.Configuration;

public sealed class FaissSyncRetrySettingsConfiguration : BaseConfiguration, IPollyRetrySettings
{
    public static readonly string Key = GetKey(nameof(FaissSyncRetrySettingsConfiguration));
    public int? TimeoutInSeconds { get; init; }
    public int? TotalAttempts { get; init; }
    public int? DelayBetweenAttemptsInSeconds { get; init; }
    public bool? UseJitter => false;
    public bool UseRetry { get; init; }
    public required FaissSyncBatchSettingsConfiguration BatchSettings { get; init; }
}