using AiTrainer.Web.Workflow.Models;

namespace AiTrainer.Web.Workflow.Activities
{
    public interface IActivity
    {
        public string Description { get; }
        Task<ActivityResultEnum> ExecuteAsync();
    }
}
