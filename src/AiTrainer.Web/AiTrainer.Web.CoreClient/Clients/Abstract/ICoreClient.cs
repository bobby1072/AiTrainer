namespace AiTrainer.Web.CoreClient.Clients.Abstract
{
    internal interface ICoreClient<TParam, TReturn>
        where TReturn : class
    {
        Task<TReturn> InvokeAsync(TParam param);
        Task<TReturn?> TryInvokeAsync(TParam param);
    }

    internal interface ICoreClient<TReturn>
        where TReturn : class
    {
        Task<TReturn> InvokeAsync();
        Task<TReturn?> TryInvokeAsync();
    }
}
