
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.EntityFramework.Entities;
using AiTrainer.Web.Persistence.Models;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Concrete;

namespace AiTrainer.Web.Domain.Services.Common.Workflow.Activities
{
    internal class SaveDomainModelToDbActivity<TModel, TModelId, TEnt, TEntId> : BaseActivity<TModel, DbResult<TModel>>
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : DomainModel<TModelId>
    {
        public override string Description => "This activity is intended to save a entity to the database";

        public override Task<(ActivityResultEnum ActivityResult, TModel? ActualResult)> ExecuteAsync(TEnt? workflowContextItem)
        {
            throw new NotImplementedException();
        }
    }
}
