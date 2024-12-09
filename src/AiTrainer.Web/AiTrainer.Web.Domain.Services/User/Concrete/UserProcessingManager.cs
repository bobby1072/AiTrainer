using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.User.Concrete
{
    public class UserProcessingManager : BaseDomainService, IUserProcessingManager
    {
        private readonly IRepository<UserEntity, Guid, Models.User> _repo;
        private readonly ILogger<UserProcessingManager> _logger;
        private readonly IValidator<Models.User> _userValidator;
        private readonly ICachingService _cachingService;
        private readonly ISolicitedDeviceTokenRepository _solicitedDeviceTokenRepository;

        public UserProcessingManager(
            IDomainServiceActionExecutor domainServiceActionExecutor,
            IApiRequestHttpContextService apiRequestService,
            IRepository<UserEntity, Guid, Models.User> repo,
            ILogger<UserProcessingManager> logger,
            IValidator<Models.User> userValidator,
            ICachingService cachingService,
            ISolicitedDeviceTokenRepository solicitedDeviceTokenRepository
        )
            : base(domainServiceActionExecutor, apiRequestService)
        {
            _repo = repo;
            _logger = logger;
            _userValidator = userValidator;
            _cachingService = cachingService;
            _solicitedDeviceTokenRepository = solicitedDeviceTokenRepository;
        }

        public async Task<Models.User> FindAndCacheUser(Guid deviceToken)
        {
            var correlationId = _apiRequestHttpContextService.CorrelationId;

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(FindAndCacheUser),
                correlationId
            );
            var foundCachedUser = await TryGetUserFromCache(deviceToken);

            if (foundCachedUser is not null)
            {
                _logger.LogInformation(
                    "User with id {UserId} found in cache for correlationId {CorrelationId} and deviceToken {DeviceToken}",
                    foundCachedUser.Id,
                    correlationId,
                    deviceToken
                );
                _logger.LogInformation(
                    "Exiting {Action} for correlationId {CorrelationId}",
                    nameof(FindAndCacheUser),
                    correlationId
                );
                return foundCachedUser;
            }

            var foundUserFromDb = await EntityFrameworkUtils.TryDbOperation(
                () => _repo.GetOne(deviceToken),
                _logger
            );

            if (foundUserFromDb?.Data is not null)
            {
                await _cachingService.SetObject(GetCacheKey(deviceToken), foundUserFromDb.Data);
                _logger.LogInformation(
                    "User with id {UserId} found in db for correlationId {CorrelationId} and deviceToken {DeviceToken}",
                    foundUserFromDb.Data,
                    correlationId,
                    deviceToken
                );
                _logger.LogInformation(
                    "Exiting {Action} for correlationId {CorrelationId}",
                    nameof(FindAndCacheUser),
                    correlationId
                );
                return foundUserFromDb.Data;
            }
            else
            {
                throw new ApiException(
                    ExceptionConstants.NotAuthorized,
                    HttpStatusCode.Unauthorized
                );
            }
        }

        public async Task<Models.SolicitedDeviceToken> IssueDeviceToken()
        {
            var correlationId = _apiRequestHttpContextService.CorrelationId;

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(IssueDeviceToken),
                correlationId
            );

            var solicitedDeviceToken = new Models.SolicitedDeviceToken
            {
                InUse = false,
                SolicitedAt = DateTime.UtcNow,
            };

            var createdEntity = await EntityFrameworkUtils.TryDbOperation(
                () => _solicitedDeviceTokenRepository.Create([solicitedDeviceToken]),
                _logger
            );

            if (
                createdEntity?.IsSuccessful is true
                && createdEntity?.Data.FirstOrDefault() is Models.SolicitedDeviceToken createdToken
                && createdToken.Id is not null
                && createdToken.Id != Guid.Empty
            )
            {
                _logger.LogInformation(
                    "Exiting {Action} successfully for correlationId {CorrelationId}",
                    nameof(IssueDeviceToken),
                    correlationId
                );
                return createdToken;
            }
            else
            {
                throw new ApiException(
                    "Failed to issue device token",
                    HttpStatusCode.InternalServerError
                );
            }
        }

        public Task<Models.User?> TryGetUserFromCache(Guid deviceToken)
        {
            _logger.LogInformation(
                "Attempting to retrieve a user for correlation id {CorrelationId} and access token {AccessToken}",
                _apiRequestHttpContextService.CorrelationId,
                deviceToken
            );

            return _cachingService.TryGetObject<Models.User>(GetCacheKey(deviceToken));
        }

        private static string GetCacheKey(Guid deviceToken) => $"{_cacheKey}{deviceToken}";

        private const string _cacheKey = "cacheUser-";
    }
}
