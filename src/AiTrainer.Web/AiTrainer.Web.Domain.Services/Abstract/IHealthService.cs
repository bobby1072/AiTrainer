namespace AiTrainer.Web.Domain.Services.Abstract;

public interface IHealthService: IDomainService
{
    Task<Models.AiTrainerHealth> GetHealth();
}