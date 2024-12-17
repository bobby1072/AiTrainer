using AiTrainer.Web.Common.Models.Configuration;

namespace AiTrainer.Web.Api.Models;

public class HealthResponse: ApplicationSettingsConfiguration 
{
    public DateTime LocalTime => DateTime.Now.ToLocalTime();
}