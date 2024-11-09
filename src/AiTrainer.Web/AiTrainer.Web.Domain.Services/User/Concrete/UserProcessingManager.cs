using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.User.Concrete
{
    internal class UserProcessingManager : IUserProcessingManager
    {
        private IRepository<UserEntity, Guid, Models.User> _repo;
        private IUserInfoClient _userInfoClient;
        private ILogger<UserProcessingManager> _logger;
        public UserProcessingManager(IRepository<UserEntity, Guid, Models.User> repo, IUserInfoClient userInfoClient, ILogger<UserProcessingManager> logger)
        {
            _repo = repo;
            _userInfoClient = userInfoClient;
            _logger = logger;
        }

        public async Task<Models.User> SaveUser(string accessToken)
        {
            var userInfo = await _userInfoClient.TryInvokeAsync(accessToken) ?? throw new ApiException("Can't get user info", HttpStatusCode.InternalServerError);

            var defaultUsername = await GetUniqueUsername();

            var user = new Models.User
            {
                Id = Guid.NewGuid(),
                Email = userInfo.Email,
                Name = userInfo.Name,
                Username = defaultUsername,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            user.ApplyCreationDefaults();
            var saveUser = await EntityFrameworkUtils.TryDbOperation(() => _repo.Create([user]), _logger) ?? throw new ApiException("Failed to create user", HttpStatusCode.InternalServerError);

            if (!saveUser.IsSuccessful)
            {
                throw new ApiException("Failed to create user", HttpStatusCode.InternalServerError);
            }

            return user;
        }

        private async Task<string> GetUniqueUsername()
        {
            for (int i = 0; i < 2; i++)
            {
                var userName = $"{Faker.Internet.UserName()}{new Random().Next(1000, 9999)}";
                var userExists = await EntityFrameworkUtils.TryDbOperation(() => _repo.Exists(userName, nameof(UserEntity.Username)), _logger);

                if (userExists?.Data == false)
                {
                    return userName;
                }

            }

            return $"{Faker.Internet.UserName()}{Guid.NewGuid().ToString().Replace("-", "")}";
        }
    }
}
