using AiTrainer.Web.Workflow.Models;

namespace AiTrainer.Web.Workflow.Contexts
{
    public abstract class BaseWorkflowOutputContext
    {
        public IList<CompletedActivityResult> CompletedActivites { get; } = new List<CompletedActivityResult>();
    }
}
