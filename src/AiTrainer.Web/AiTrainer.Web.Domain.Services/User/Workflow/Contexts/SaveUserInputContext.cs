using BT.Common.WorkflowActivities.Contexts;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Contexts
{
    internal class SaveUserInputContext: IWorkflowInputContext
    {
        public Models.User NewUser {  get; set; }
    }
}