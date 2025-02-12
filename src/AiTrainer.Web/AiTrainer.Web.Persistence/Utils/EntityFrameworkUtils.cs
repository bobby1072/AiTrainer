using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Utils
{
    public static class EntityFrameworkUtils
    {
        public static async Task<TResult?> TryDbOperation<TResult>(
            Func<Task<TResult>> dbOperation,
            ILogger<object>? logger = null
        )
            where TResult : class
        {
            try
            {
                return await dbOperation.Invoke();
            }
            catch (Exception ex)
            {
                logger?.LogError(
                    ex,
                    "Exception occurred during db operation with message {Message}",
                    ex.Message
                );

                return null;
            }
        }

        public static TResult? TryDbOperation<TResult>(
            Func<TResult> dbOperation,
            ILogger<object>? logger = null
        )
            where TResult : class
        {
            try
            {
                return dbOperation.Invoke();
            }
            catch (Exception ex)
            {
                logger?.LogError(
                    ex,
                    "Exception occurred during db operation with message {Message}",
                    ex.Message
                );

                return null;
            }
        }
    }
}
