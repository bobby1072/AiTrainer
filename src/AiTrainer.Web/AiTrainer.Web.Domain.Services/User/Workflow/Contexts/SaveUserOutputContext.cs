using BT.Common.WorkflowActivities.Contexts;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Contexts
{
    internal class SaveUserOutputContext: WorkflowOutputContext<Models.User>
    {
        public override Models.User? ReturnObject { get; set; }
    }
}
