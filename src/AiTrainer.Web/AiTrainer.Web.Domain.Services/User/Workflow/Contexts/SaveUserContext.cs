using BT.Common.WorkflowActivities.Contexts;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Contexts
{
    internal class SaveUserContext: WorkflowContext<SaveUserInputContext, SaveUserOutputContext, Models.User>
    {
        public override SaveUserInputContext Input { get; init; } = new SaveUserInputContext();
        public override SaveUserOutputContext Output { get; init; } = new SaveUserOutputContext();
    }
}
