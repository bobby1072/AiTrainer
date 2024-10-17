using AiTrainer.Web.Common;
using AiTrainer.Web.CoreClient.Exceptions;

namespace AiTrainer.Web.CoreClient.Extensions
{
    internal static class HttpResponseMessageExceptions
    {
        public static void EnsureSuccessStatusCodeAndThrowCoreClientException(
            this HttpResponseMessage response
        )
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                throw new CoreClientException(ExceptionConstants.InternalServerError);
            }
        }
    }
}
