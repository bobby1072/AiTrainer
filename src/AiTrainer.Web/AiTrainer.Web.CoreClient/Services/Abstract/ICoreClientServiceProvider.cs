namespace AiTrainer.Web.CoreClient.Services.Abstract
{
    public interface ICoreClientServiceProvider
    {
        public Task<TReturn> ExecuteAsync<TReturn>()
            where TReturn : class;
        public Task<TReturn> ExecuteAsync<TParam, TReturn>(TParam requestBody)
            where TReturn : class;
        public Task<TReturn?> TryExecuteAsync<TReturn>()
            where TReturn : class;
        public Task<TReturn?> TryExecuteAsync<TParam, TReturn>(TParam requestBody)
            where TReturn : class;
    }
}
