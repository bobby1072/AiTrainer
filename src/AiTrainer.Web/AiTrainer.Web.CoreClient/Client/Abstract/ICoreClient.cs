namespace AiTrainer.Web.CoreClient.Client.Abstract
{
    public interface ICoreClient<TParam, TReturn> where TReturn: class
    {
        Task<TReturn> InvokeAsync(TParam? param);
        Task<TReturn?> TryInvokeAsync(TParam? param);
    }
}
