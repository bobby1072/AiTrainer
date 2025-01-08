using AiTrainer.Web.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace AiTrainer.Web.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireUserLoginAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                if (context.HttpContext.Request.Headers[HeaderNames.Authorization].FirstOrDefault() is null)
                {
                    throw new ApiException(
                        ExceptionConstants.NotAuthorized,
                        HttpStatusCode.Unauthorized
                    );
                }
                return;
            }
            context.Result = new UnauthorizedResult();
        }
    }
}
