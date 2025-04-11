using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;

namespace AiTrainer.Web.CoreClient.Clients.Abstract
{
    public interface ICoreClient { }
    public interface ICoreClient<in TParam, TReturn>: ICoreClient
        where TParam : class
        where TReturn : class
    {
        Task<TReturn?> TryInvokeAsync(TParam param, CancellationToken cancellation = default);
    }

    public interface ICoreClient<TReturn>: ICoreClient
        where TReturn : class
    {
        Task<TReturn?> TryInvokeAsync(CancellationToken cancellation = default);
    }
}
