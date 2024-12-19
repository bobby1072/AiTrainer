using AiTrainer.Web.Api.Models;

namespace AiTrainer.Web.Domain.Services.Abstract;

public interface IHealthService
{
    Task<AiTrainerHealth> GetHealth();
}