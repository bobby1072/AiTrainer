using AiTrainer.Web.Workflow.Models;

namespace AiTrainer.Web.Workflow.Activities.Abstract
{
    public interface IActivity
    {
        public string Description {  get; }
        Task<ActivityResultEnum> ExecuteAsync();
    }
}
