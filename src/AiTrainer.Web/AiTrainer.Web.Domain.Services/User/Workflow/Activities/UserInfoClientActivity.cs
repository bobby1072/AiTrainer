using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using AiTrainer.Web.UserInfoClient.Models;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Attributes;
using BT.Common.WorkflowActivities.Activities.Concrete;
using BT.Common.WorkflowActivities.Contexts;

namespace AiTrainer.Web.Domain.Services.User.Workflow.Activities
{
    [DefaultActivityRetry(2, 1)]
    internal class UserInfoClientActivity: BaseActivity<UserInfoClientActivityContextItem, UserInfoClientActivityReturnItem>
    {
        public override string Description => "This activity is used to make a request to the user info client with a access token";
        private readonly IUserInfoClient _userInfoClient;
        public UserInfoClientActivity(IUserInfoClient userInfoClient)
        {
            _userInfoClient = userInfoClient;
        }
        public override async Task<(ActivityResultEnum ActivityResult, UserInfoClientActivityReturnItem ActualResult)> ExecuteAsync(UserInfoClientActivityContextItem workflowContextItem)
        {
            if (string.IsNullOrEmpty(workflowContextItem.AccessToken))
            {
                return (ActivityResultEnum.Skip, new UserInfoClientActivityReturnItem());
            }

            var userInfo = await _userInfoClient.TryInvokeAsync(workflowContextItem.AccessToken);

            return (userInfo is null ? ActivityResultEnum.Fail: ActivityResultEnum.Success, new UserInfoClientActivityReturnItem { UserInfoResponse = userInfo});
        }
    }
    internal record UserInfoClientActivityContextItem: ActivityContextItem
    {
        public string? AccessToken { get; init; }
    }
    internal record UserInfoClientActivityReturnItem: ActivityReturnItem
    {
        public UserInfoResponse? UserInfoResponse { get; init; }
    }
}