using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Domain.Services.Concrete
{
    public class ApiRequestHttpContextService : IApiRequestHttpContextService
    {

        private HttpContext? _httpContext;
        public ApiRequestHttpContextService(HttpContext? httpContext)
        {
            _httpContext = httpContext;
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

        private Guid? _deviceToken;
        public Guid DeviceToken
        {
            get
            {
                _deviceToken ??= _httpContext.GetDeviceToken();

                return (Guid)_deviceToken!;
            }
        }
    }
}
