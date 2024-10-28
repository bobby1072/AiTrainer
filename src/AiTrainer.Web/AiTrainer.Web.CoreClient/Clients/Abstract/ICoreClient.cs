namespace AiTrainer.Web.CoreClient.Clients.Abstract
{
    public interface ICoreClient<TParam, TReturn>
        where TReturn : class
    {
        Task<TReturn> InvokeAsync(TParam param);
        Task<TReturn?> TryInvokeAsync(TParam param);
    }

    public interface ICoreClient<TReturn>
        where TReturn : class
    {
        Task<TReturn> InvokeAsync();
        Task<TReturn?> TryInvokeAsync();
    }
}
