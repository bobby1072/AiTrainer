namespace AiTrainer.Web.Common.Services.Abstract
{
    public interface IApiResolverService
    {
        T GetService<T>()
            where T : class;
    }
}
