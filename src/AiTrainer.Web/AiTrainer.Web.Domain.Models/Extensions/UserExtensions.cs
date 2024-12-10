using AiTrainer.Web.Common.Models.ApiModels.Request;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class UserExtensions
    {
        public static User ToUserModel(
            this SaveUserInput userInput,
            DateTime createdAt,
            DateTime dateModified,
            Guid oldId
        )
        {
            return new User
            {
                DateCreated = createdAt,
                DateModified = dateModified,
                Id = userInput.NewDeviceToken is null ? oldId : userInput.NewDeviceToken,
                Email = userInput.Email,
                Name = userInput.Name,
            };
        }

        public static User ToNewUserModel(this SaveUserInput userInput, Guid oldId)
        {
            return new User
            {
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                Id = oldId,
                Email = userInput.Email,
                Name = userInput.Name,
            };
        }
    }
}
