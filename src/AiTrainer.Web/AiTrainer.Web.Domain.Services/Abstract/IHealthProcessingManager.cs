namespace AiTrainer.Web.Domain.Services.Abstract;

public interface IHealthProcessingManager: IDomainProcessingManager
{
    Task<Models.AiTrainerHealth> GetHealth();
}