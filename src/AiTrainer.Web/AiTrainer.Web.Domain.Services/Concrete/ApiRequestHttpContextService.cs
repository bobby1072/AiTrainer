using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Domain.Services.Concrete
{
    public class ApiRequestHttpContextService : IApiRequestHttpContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiRequestHttpContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpContext HttpContext => _httpContextAccessor.HttpContext;
        private Guid? _correlationId;
        public Guid? CorrelationId
        {
            get
            {
                _correlationId ??= HttpContext.GetCorrelationId();

                return _correlationId;
            }
        }

        private Guid? _deviceToken;
        public Guid DeviceToken
        {
            get
            {
                _deviceToken ??= HttpContext.GetDeviceToken();

                return (Guid)_deviceToken!;
            }
        }
    }
}
