using BT.Common.Helpers.TypeFor;
using BT.Common.WorkflowActivities.Abstract;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Concrete;
using BT.Common.WorkflowActivities.Contexts;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Concrete
{
    // internal class SaveUserWorkflow : BaseWorkflow<SaveUserContext, SaveUserInputContext, SaveUserOutputContext, Models.User>
    // {
    //     public override string Description => "This workflow is used to edit or create a domain user in the db. Including from an access token";
    // }

    internal record SaveUserContext : WorkflowContext<SaveUserInputContext, SaveUserOutputContext, Models.User>
    {
        public override SaveUserInputContext Input { get; init; } = new SaveUserInputContext();
        public override SaveUserOutputContext Output { get; init; } = new SaveUserOutputContext();
    }
    internal record SaveUserInputContext : WorkflowInputContext
    {
        public Models.User? NewUser { get; set; }
        public Models.User? FoundSameUser { get; set; }

        public string? AccessToken { get; set; }
    }
    internal record SaveUserOutputContext : WorkflowOutputContext<Models.User>
    {
        public override Models.User? ReturnObject { get; set; }
    }
}
