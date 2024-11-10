using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.User.Concrete
{
    internal class UserProcessingManager : IUserProcessingManager
    {
        private readonly IRepository<UserEntity, Guid, Models.User> _repo;
        private readonly IUserInfoClient _userInfoClient;
        private readonly ILogger<UserProcessingManager> _logger;
        private readonly IValidator<Models.User> _userValidator;
        public UserProcessingManager(IRepository<UserEntity, Guid, Models.User> repo, IUserInfoClient userInfoClient, ILogger<UserProcessingManager> logger, IValidator<Models.User> userValidator)
        {
            _repo = repo;
            _userInfoClient = userInfoClient;
            _logger = logger;
            _userValidator = userValidator;
        }

        public async Task<Models.User> SaveUserIfDoesNotExist(string accessToken)
        {

            var userInfo = await _userInfoClient.TryInvokeAsync(accessToken) ?? throw new ApiException("Can't get user info", HttpStatusCode.InternalServerError);

            var foundUserFromDb = await EntityFrameworkUtils.TryDbOperation(() => _repo.GetOne(userInfo.Email, nameof(UserEntity.Email)), _logger);

            if (foundUserFromDb?.Data is not null)
            {
                return foundUserFromDb.Data;
            }

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

            var validationResult = await _userValidator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                throw new ApiException("Failed to create user", HttpStatusCode.InternalServerError);
            }

            var saveUser = await EntityFrameworkUtils.TryDbOperation(() => _repo.Create([user]), _logger) ?? throw new ApiException("Failed to create user", HttpStatusCode.InternalServerError);

            if (!saveUser.IsSuccessful)
            {
                throw new ApiException("Failed to create user", HttpStatusCode.InternalServerError);
            }

            return saveUser.Data.First();
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
