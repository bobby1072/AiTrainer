namespace AiTrainer.Web.Domain.Services.User
{
    public interface IUserProcessingManager
    {
        Task<Models.User> SaveUser(string accessToken);
    }
}