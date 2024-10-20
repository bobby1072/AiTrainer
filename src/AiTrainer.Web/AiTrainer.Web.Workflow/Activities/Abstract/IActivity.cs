using AiTrainer.Web.Workflow.Models;

namespace AiTrainer.Web.Workflow.Activities
{
    public interface IActivity<T>
    {
        public string Name { get; }
        public string Description { get; }
        Task<ActivityResultEnum> ExecuteAsync(T workflowContexItem);
    }
}
