namespace AiTrainer.Web.Domain.Services.Abstract;

public interface IHealthProcessingManager: IDomainProcessingManager
{
    Task<Domain.Models.AiTrainerHealth> GetHealth();
}