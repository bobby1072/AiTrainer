using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;

namespace AiTrainer.Web.CoreClient.Clients.Abstract
{
    public interface ICoreClient<in TParam, TReturn>
        where TParam : BaseCoreClientRequestBody
        where TReturn : BaseCoreClientResponseBody
    {
        Task<TReturn?> TryInvokeAsync(TParam param);
    }

    public interface ICoreClient<TReturn>
        where TReturn : BaseCoreClientResponseBody
    {
        Task<TReturn?> TryInvokeAsync();
    }
}
