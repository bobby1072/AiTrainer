using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Domain.Services.Concrete
{
    public class ApiRequestHttpContextService: IApiRequestHttpContextService
    {
        private readonly HttpContext? _httpContext;
        public ApiRequestHttpContextService(HttpContext? httpContextAccessor)
        {
            _httpContext = httpContextAccessor;
        }
        private Guid? _correlationId;
        public Guid? CorrelationId
        {
            get
            {
                _correlationId ??= _httpContext?.GetCorrelationId();

                return _correlationId;
            }
        }

        private string? _accessToken;
        public string AccessToken
        {
            get
            {
                _accessToken ??= _httpContext?.GetAccessToken() ?? throw new ApiException(
                    ExceptionConstants.NotAuthorized,
                    HttpStatusCode.Unauthorized
                );

                return _accessToken;
            }
        }
    }
}
