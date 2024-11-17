namespace AiTrainer.Web.Domain.Services.Hangfire
{
    internal static class HangfireConstants
    {
        internal static class Queues
        {
            public const string BuildFaissStoreQueue = "build_faiss_store_queue";

            public static readonly string[] FullQueueList = [BuildFaissStoreQueue];
        }
    }
}
