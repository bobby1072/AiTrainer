namespace AiTrainer.Web.CoreClient.Client.Abstract
{
    public interface ICoreClient<TReturn, TParam>
    {
        Task<TReturn> InvokeAsync(TParam? param);
        Task<TReturn?> TryInvokeAsync(TParam? param);
    }
}
