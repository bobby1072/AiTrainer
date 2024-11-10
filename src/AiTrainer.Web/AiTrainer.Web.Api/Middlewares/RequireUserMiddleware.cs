using System.Net;
using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.User.Abstract;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class RequireUserMiddleware : BaseMiddleware
    {
        private readonly IDomainServiceActionExecutor _domainServiceExecutor;
        private readonly ILogger<RequireUserMiddleware> _logger;
        private readonly ICachingService _cachingService;

        public RequireUserMiddleware(
            RequestDelegate next,
            IDomainServiceActionExecutor domainServiceExecutor,
            ILogger<RequireUserMiddleware> logger,
            ICachingService cachingService
        )
            : base(next)
        {
            _domainServiceExecutor = domainServiceExecutor;
            _logger = logger;
            _cachingService = cachingService;
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            if (context.GetEndpoint()?.Metadata.GetMetadata<RequireUserAttribute>() is not null)
            {
                var correlationId = context.GetCorrelationId();
                _logger.LogInformation(
                    "----Entering {RequireUserMiddleware} for correlationId {CorrelationId}----",
                    nameof(RequireUserMiddleware),
                    correlationId
                );
                var accessToken =
                    context.Request.Headers.Authorization.First()
                    ?? throw new ApiException(
                        ExceptionConstants.NotAuthorized,
                        HttpStatusCode.Unauthorized
                    );

                var foundUser = await _cachingService.TryGetObject<User>(
                    User.GetCacheKey(accessToken)
                );

                if (foundUser is not null)
                {
                    _logger.LogInformation(
                        "User with id {UserId} and username {Username} found in cache for correlationId {CorrelationId} and accessToken {AccessToken}",
                        foundUser.Id,
                        foundUser.Username,
                        correlationId,
                        accessToken
                    );
                    _logger.LogInformation(
                        "----Exiting {RequireUserMiddleware} for correlationId {CorrelationId}----",
                        nameof(RequireUserMiddleware),
                        correlationId
                    );
                    await _next.Invoke(context);
                    return;
                }

                var newOrDbFoundUser = await _domainServiceExecutor.ExecuteAsync<
                    IUserProcessingManager,
                    User
                >((userService) => userService.SaveUserIfDoesNotExist(accessToken));

                await _cachingService.SetObject(
                    User.GetCacheKey(accessToken),
                    newOrDbFoundUser,
                    CacheObjectTimeToLiveInSeconds.ThirtyMinutes
                );
                _logger.LogInformation(
                    "User with id {UserId} and username {Username} has been cached for correlationId {CorrelationId} and accessToken {AccessToken}",
                    newOrDbFoundUser.Id,
                    newOrDbFoundUser.Username,
                    correlationId,
                    accessToken
                );
                _logger.LogInformation(
                    "----Exiting {RequireUserMiddleware} for correlationId {CorrelationId}----",
                    nameof(RequireUserMiddleware),
                    correlationId
                );
            }
            await _next.Invoke(context);
        }
    }
}
