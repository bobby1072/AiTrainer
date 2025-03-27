namespace AiTrainer.Web.Domain.Models;

public class AiTrainerHealth: DomainModel<AiTrainerHealth>
{
    public required bool IsCoreHealthy { get; init; }
    public DateTime LocalTime => DateTime.Now.ToLocalTime();
    public required string Name { get; init; }   
    public required string ReleaseVersion { get; init; }

    public override bool Equals(AiTrainerHealth? obj)
    {
        return obj?.IsCoreHealthy == IsCoreHealthy
            && LocalTime == obj.LocalTime
            && Name == obj.Name
            && ReleaseVersion == obj.ReleaseVersion;
    }
}