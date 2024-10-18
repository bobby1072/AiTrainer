using AiTrainer.Web.Common;
using AiTrainer.Web.CoreClient.Exceptions;
using AiTrainer.Web.CoreClient.Models.Response;

namespace AiTrainer.Web.CoreClient.Extensions
{
    internal static class CoreResponseExtensions
    {
        public static T EnsureSuccessfulCoreResponseAndGetData<T>(
            this CoreResponse<T>? coreResponse
        )
        {
            if (coreResponse is null)
            {
                throw new CoreClientException(ExceptionConstants.InternalServerError);
            }
            EnsureSuccessfulResponse(coreResponse);

            if (coreResponse.Data is null)
            {
                throw new CoreClientException(
                    !string.IsNullOrEmpty(coreResponse.ExceptionMessage)
                        ? coreResponse.ExceptionMessage
                        : ExceptionConstants.InternalServerError
                );
            }

            return coreResponse.Data;
        }

        public static void EnsureSuccessfulResponse<T>(this CoreResponse<T>? coreResponse)
        {
            if (coreResponse is null)
            {
                throw new CoreClientException(ExceptionConstants.InternalServerError);
            }
            if (!coreResponse.IsSuccess)
            {
                throw new CoreClientException(
                    coreResponse.ExceptionMessage is not null
                        ? coreResponse.ExceptionMessage
                        : ExceptionConstants.InternalServerError
                );
            }
        }
    }
}
