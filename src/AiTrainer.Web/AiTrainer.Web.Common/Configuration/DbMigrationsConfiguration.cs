using BT.Common.Polly.Models.Abstract;

namespace AiTrainer.Web.Common.Configuration;

public sealed class DbMigrationsConfiguration: BaseConfiguration, IPollyRetrySettings
{
    public static readonly string Key = GetKey(nameof(DbMigrationsConfiguration));
    public required string StartVersion { get; init; } 
    public int? TimeoutInSeconds { get; init; }
    public int? TotalAttempts { get; init; }
    public int? DelayBetweenAttemptsInSeconds { get; init; }
    public bool? UseJitter { get; init; }
}