namespace AiTrainer.Web.Common.Configuration;

public sealed class FaissSyncBatchSettingsConfiguration: BaseConfiguration
{
    public static readonly string Key = $"{FaissSyncRetrySettingsConfiguration.Key}:BatchSettings";
    public bool UseBatching { get; init; }
    public int BatchSize { get; init; }
    public int BatchExecutionIntervalInSeconds { get; init; }
}