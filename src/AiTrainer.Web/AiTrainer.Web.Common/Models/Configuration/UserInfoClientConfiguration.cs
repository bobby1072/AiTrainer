namespace AiTrainer.Web.Common.Models.Configuration;

public class UserInfoClientConfiguration : BaseConfiguration
{
    public static readonly string Key = GetKey(nameof(UserInfoClientConfiguration));
    
    public required int TimeoutInSeconds { get; init; }
    public required int TotalAttempts { get; init; }
    public required int DelayBetweenAttemptsInSeconds { get; init; }
}