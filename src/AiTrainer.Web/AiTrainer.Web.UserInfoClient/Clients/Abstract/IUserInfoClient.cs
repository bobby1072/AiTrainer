using AiTrainer.Web.UserInfoClient.Models;

namespace AiTrainer.Web.UserInfoClient.Clients.Abstract
{
    public interface IUserInfoClient
    {
        Task<UserInfoResponse> InvokeAsync(string accessToken);
        Task<UserInfoResponse?> TryInvokeAsync(string accessToken);
    }
}
