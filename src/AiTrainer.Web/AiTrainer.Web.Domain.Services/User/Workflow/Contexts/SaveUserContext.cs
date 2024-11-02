using BT.Common.WorkflowActivities.Contexts;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Contexts
{
    internal class SaveUserContext: IWorkflowContext<SaveUserInputContext, SaveUserOutputContext, Models.User>
    {
        public SaveUserInputContext Input { get; init; } = new SaveUserInputContext();
        public SaveUserOutputContext Output { get; init; } = new SaveUserOutputContext();
    }
}
