using BT.Common.Polly.Models.Abstract;

namespace AiTrainer.Web.Common.Models.Configuration;

public class UserInfoClientConfiguration : BaseConfiguration, IPollyRetrySettings
{
    public static readonly string Key = GetKey(nameof(UserInfoClientConfiguration));
    
    public int? TimeoutInSeconds { get; init; }
    public int? TotalAttempts { get; init; }
    public int? DelayBetweenAttemptsInSeconds { get; init; }
    public bool? UseJitter => false;
}