using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Hangfire.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.Hangfire.Concrete
{
    internal class HangfireJobService : IHangfireJobService
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ILogger<HangfireJobService> _logger;

        public HangfireJobService(
            IBackgroundJobClient backgroundJobs,
            IRecurringJobManager recurringJobManager,
            ILogger<HangfireJobService> logger
        )
        {
            _backgroundJobs = backgroundJobs;
            _recurringJobManager = recurringJobManager;
            _logger = logger;
        }

        public void RegisterJobs()
        {
            RegisterHourlyJobs();
            RegisterDailyJobs();
            RegisterWeeklyJobs();
            RegisterMonthlyJobs();
            RegisterStartupJobs();
        }

        public void RegisterHourlyJobs()
        {
            _recurringJobManager.RemoveIfExists(HangfireConstants.JobNames.CleanUpExpiredDeviceTokens);
            
            _recurringJobManager.AddOrUpdate<IUserProcessingManager>(
                HangfireConstants.JobNames.CleanUpExpiredDeviceTokens,
                HangfireConstants.Queues.CleanerQueue,
                x => x.CleanUpDeviceTokens(),
                Cron.Hourly
            );
        }

        private void RegisterDailyJobs() { }
        private void RegisterWeeklyJobs() { }
        private void RegisterMonthlyJobs() { }
        private void RegisterStartupJobs() { }
    }
}
