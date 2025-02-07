namespace AiTrainer.Web.Domain.Models;

public record AiTrainerHealth
{
    public required bool IsCoreHealthy { get; init; }
    public DateTime LocalTime => DateTime.Now.ToLocalTime();
    public required string Name { get; init; }   
    public required string ReleaseVersion { get; init; }
}