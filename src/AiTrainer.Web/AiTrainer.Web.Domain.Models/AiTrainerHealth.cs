using AiTrainer.Web.Common.Models.Configuration;

namespace AiTrainer.Web.Api.Models;

public class AiTrainerHealth: ApplicationSettingsConfiguration 
{
    public required bool IsCoreHealthy { get; init; }
    public DateTime LocalTime => DateTime.Now.ToLocalTime();
}