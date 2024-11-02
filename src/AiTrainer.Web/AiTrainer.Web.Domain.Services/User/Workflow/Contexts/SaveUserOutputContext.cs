using BT.Common.WorkflowActivities.Contexts;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Contexts
{
    internal class SaveUserOutputContext: IWorkflowOutputContext<Models.User>
    {
        public Models.User? ReturnObject { get; set; }
    }
}
