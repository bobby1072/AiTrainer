﻿using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using AiTrainer.Web.UserInfoClient.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.User.Concrete
{
    internal sealed class UserProcessingManager : IUserProcessingManager
    {
        private readonly IRepository<UserEntity, Guid, Domain.Models.User> _repo;
        private readonly IUserInfoClient _userInfoClient;
        private readonly ILogger<UserProcessingManager> _logger;
        private readonly IValidator<Domain.Models.User> _userValidator;
        private readonly ICachingService _cachingService;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private const string _cacheKey = "cacheUser-";

        public UserProcessingManager(
            IHttpContextAccessor? httpContextAccessor,
            IRepository<UserEntity, Guid, Domain.Models.User> repo,
            IUserInfoClient userInfoClient,
            ILogger<UserProcessingManager> logger,
            IValidator<Domain.Models.User> userValidator,
            ICachingService cachingService
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _repo = repo;
            _userInfoClient = userInfoClient;
            _logger = logger;
            _userValidator = userValidator;
            _cachingService = cachingService;
        }

        public async Task<Domain.Models.User> SaveAndCacheUser(string accessToken)
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(SaveAndCacheUser),
                correlationId
            );
            var foundCachedUser = await TryGetUserFromCache(accessToken);

            if (foundCachedUser is not null)
            {
                _logger.LogInformation(
                    "User with id {UserId} found in cache for correlationId {CorrelationId} and accessToken {AccessToken}",
                    foundCachedUser.Id,
                    correlationId,
                    accessToken
                );
                _logger.LogInformation(
                    "Exiting {Action} for correlationId {CorrelationId}",
                    nameof(SaveAndCacheUser),
                    correlationId
                );
                return foundCachedUser;
            }

            var (userInfo, foundUserFromDb) = await GetUserInfoAndDbUser(accessToken);

            if (
                foundUserFromDb?.Data is not null
                && foundUserFromDb.Data.Email == userInfo.Email
                && foundUserFromDb.Data.Name == userInfo.Name
            )
            {
                await _cachingService.SetObject(GetCacheKey(accessToken), foundUserFromDb.Data);
                _logger.LogInformation(
                    "User with id {UserId} has been cached for correlationId {CorrelationId} and accessToken {AccessToken}",
                    foundUserFromDb.Data.Id,
                    correlationId,
                    accessToken
                );
                _logger.LogInformation(
                    "Exiting {Action} for correlationId {CorrelationId}",
                    nameof(SaveAndCacheUser),
                    correlationId
                );
                return foundUserFromDb.Data;
            }
            var userDto = GetSaveUserDto(userInfo, foundUserFromDb);

            var userSaveExceptionMessage =
                $"Failed to {userDto.SaveType.ToString().ToLower()} user";

            var validationResult = await _userValidator.ValidateAsync(userDto.User);

            if (!validationResult.IsValid)
            {
                throw new ApiException(
                    userSaveExceptionMessage
                );
            }
            _logger.LogInformation(
                "Attempting to {UserSaveEnum} user with id {UserId} for correlationId {CorrelationId}",
                userDto.SaveType,
                userDto.User.Id,
                correlationId
            );
            var saveUser =
                await EntityFrameworkUtils.TryDbOperation(
                    () =>
                        userDto.SaveType == UserSaveEnum.Create
                            ? _repo.Create([userDto.User])
                            : _repo.Update([userDto.User]),
                    _logger
                ) ?? throw new ApiException(userSaveExceptionMessage);

            if (!saveUser.IsSuccessful)
            {
                throw new ApiException(userSaveExceptionMessage);
            }
            var userToReturn = saveUser.Data.First();
            await _cachingService.SetObject(GetCacheKey(accessToken), userToReturn);
            _logger.LogInformation(
                "User with id {UserId} has been cached for correlationId {CorrelationId} and accessToken {AccessToken}",
                userToReturn.Id,
                correlationId,
                accessToken
            );
            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(SaveAndCacheUser),
                correlationId
            );
            return userToReturn;
        }

        public Task<Domain.Models.User?> TryGetUserFromCache(string accessToken)
        {
            _logger.LogInformation(
                "Attempting to retrieve a user for correlation id {CorrelationId}",
                _httpContextAccessor?.HttpContext?.GetCorrelationId()
            );

            return _cachingService.TryGetObject<Domain.Models.User>(GetCacheKey(accessToken));
        }

        private (Domain.Models.User User, UserSaveEnum SaveType) GetSaveUserDto(
            UserInfoResponse userInfo,
            DbGetOneResult<Domain.Models.User>? foundUserFromDb
        )
        {
            (Domain.Models.User User, UserSaveEnum SaveType) userDto;

            if (foundUserFromDb?.Data is not null)
            {
                userDto = (
                    new Domain.Models.User
                    {
                        Id = foundUserFromDb.Data.Id,
                        Email = userInfo.Email,
                        Name = userInfo.Name,
                        DateCreated = foundUserFromDb.Data.DateCreated,
                        DateModified = DateTime.UtcNow,
                    },
                    UserSaveEnum.Update
                );
            }
            else
            {
                userDto = (
                    new Domain.Models.User
                    {
                        Email = userInfo.Email,
                        Name = userInfo.Name,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                    },
                    UserSaveEnum.Create
                );
                userDto.User.ApplyCreationDefaults();
            }

            return userDto;
        }

        private async Task<(
            UserInfoResponse UserInfo,
            DbGetOneResult<Domain.Models.User>? UserFromDb
        )> GetUserInfoAndDbUser(string accessToken)
        {
            var userInfo =
                await _userInfoClient.TryInvokeAsync(accessToken)
                ?? throw new InvalidDataException("Can't get user info");

            var foundUserFromDb = await EntityFrameworkUtils.TryDbOperation(
                () => _repo.GetOne(userInfo.Email, nameof(UserEntity.Email), nameof(UserEntity.GlobalFileCollectionConfig)),
                _logger
            );

            return (userInfo, foundUserFromDb);
        }

        private static string GetCacheKey(string accessToken) => $"{_cacheKey}{accessToken}";


        private enum UserSaveEnum
        {
            Update,
            Create,
        }
    }
}
