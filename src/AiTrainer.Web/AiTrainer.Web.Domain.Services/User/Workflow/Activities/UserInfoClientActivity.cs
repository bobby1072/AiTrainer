using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using AiTrainer.Web.UserInfoClient.Models;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Concrete;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Activities
{
    public class UserInfoClientActivity: BaseActivity<string, UserInfoResponse>
    {
        public override string Description => "This activity is used to make a request to the user info client with a access token";
        private readonly IUserInfoClient _userInfoClient;
        public UserInfoClientActivity(IUserInfoClient userInfoClient)
        {
            _userInfoClient = userInfoClient;
        }
        public override async Task<(ActivityResultEnum ActivityResult, UserInfoResponse? ActualResult)> ExecuteAsync(string? workflowContextItem)
        {
            if (string.IsNullOrEmpty(workflowContextItem))
            {
                return (ActivityResultEnum.Skip, null);
            }

            var userInfo = await _userInfoClient.TryInvokeAsync(workflowContextItem);

            return (userInfo is null ? ActivityResultEnum.Fail: ActivityResultEnum.Success, userInfo);
        }
    }
}