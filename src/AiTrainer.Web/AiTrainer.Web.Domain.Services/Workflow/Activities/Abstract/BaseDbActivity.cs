using AiTrainer.Web.Persistence.Entities;

namespace AiTrainer.Web.Domain.Services.Workflow.Activities.Abstract
{
    using BT.Common.WorkflowActivities.Activities.Abstract;
    using BT.Common.WorkflowActivities.Contexts;

    internal abstract class BaseDbActivity<TModel, TEnt, TEntId, TContextItem, TReturnItem>: BaseActivity<TContextItem, TReturnItem>
        where TContextItem: ActivityContextItem 
        where TReturnItem : ActivityReturnItem
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
    {
    }
}
