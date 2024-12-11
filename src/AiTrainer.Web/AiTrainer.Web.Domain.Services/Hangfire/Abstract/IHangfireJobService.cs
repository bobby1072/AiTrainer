using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.Hangfire.Abstract
{
    public interface IHangfireJobService : IDomainService
    {
        void RegisterJobs();
    }
}
