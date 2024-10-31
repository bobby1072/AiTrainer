
using AiTrainer.Web.Persistence.EntityFramework.Entities;

namespace AiTrainer.Web.Domain.Services.Common.Workflow.Activities
{
    internal class SaveDomainModelToDbActivity<TEnt, TModel>
        where TEnt : BaseEntity<object, TModel>
        where TModel : class
    {

    }
}
