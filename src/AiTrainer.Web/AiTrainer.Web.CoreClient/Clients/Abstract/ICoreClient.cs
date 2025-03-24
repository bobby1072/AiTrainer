using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;

namespace AiTrainer.Web.CoreClient.Clients.Abstract
{
    public interface ICoreClient { }
    public interface ICoreClient<in TParam, TReturn>: ICoreClient
        where TParam : BaseCoreClientRequestData
        where TReturn : BaseCoreClientResponseData
    {
        Task<TReturn?> TryInvokeAsync(TParam param, CancellationToken cancellation = default);
    }

    public interface ICoreClient<TReturn>: ICoreClient
        where TReturn : BaseCoreClientResponseData
    {
        Task<TReturn?> TryInvokeAsync(CancellationToken cancellation = default);
    }
}
