using Microsoft.AspNetCore.Mvc.Filters;

namespace AiTrainer.Web.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireUserLoginAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context) { }
    }
}
