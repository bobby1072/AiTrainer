namespace AiTrainer.Web.Domain.Services.Hangfire
{
    internal static class HangfireConstants
    {
        internal static class JobNames
        {
            public const string CleanUpExpiredDeviceTokens = "CleanUpExpiredDeviceTokensJob";
        }

        internal static class Queues
        {
            public const string BuildFaissStoreQueue = "build_faiss_store_queue";

            public const string CleanerQueue = "cleaner_queue";
            public static readonly string[] FullQueueList = [BuildFaissStoreQueue, CleanerQueue];
        }
    }
}
