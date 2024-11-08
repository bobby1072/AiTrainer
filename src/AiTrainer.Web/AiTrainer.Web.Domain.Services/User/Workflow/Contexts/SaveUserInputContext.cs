using BT.Common.WorkflowActivities.Contexts;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Contexts
{
    internal class SaveUserInputContext: WorkflowInputContext
    {
        public Models.User? NewUser {  get; set; }
        public Models.User? FoundSameUser {  get; set; }

        public string? AccessToken { get; set; }
    }
}