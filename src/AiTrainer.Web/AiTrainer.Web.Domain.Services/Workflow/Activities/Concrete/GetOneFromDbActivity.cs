using AiTrainer.Web.Domain.Services.Workflow.Activities.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using BT.Common.WorkflowActivities.Activities.Concrete;
using BT.Common.WorkflowActivities.Contexts;

namespace AiTrainer.Web.Domain.Services.Workflow.Activities.Concrete
{
    internal class GetOneFromDbActivity<TModel, TEnt, TEntId, TSearchValue> : BaseDbActivity<TModel, TEnt, TEntId, GetOneFromDbActivityContextItem<TSearchValue>, GetOneFromDbActivityReturnItem<TModel>>
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
    {
        public override string Description => "This activity is used to get one record form the db";

        public override Task<(ActivityResultEnum ActivityResult, GetOneFromDbActivityReturnItem<TModel> ActualResult)> ExecuteAsync(GetOneFromDbActivityContextItem<TSearchValue> workflowContextItem)
        {
            throw new NotImplementedException();
        }
    }
    internal record GetOneFromDbActivityContextItem<TSearchValue> : ActivityContextItem
    {
        public required TSearchValue SearchValue { get; init; }
        public required string PropertyName { get; init; }
    }
    internal record GetOneFromDbActivityReturnItem<TModel> : ActivityReturnItem where TModel : class
    {
        public required DbGetOneResult<TModel> DbGetOneResult { get; init; }
    }
}
