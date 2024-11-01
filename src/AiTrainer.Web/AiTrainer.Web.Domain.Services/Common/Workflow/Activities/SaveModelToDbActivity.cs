using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Persistence.EntityFramework.Entities;
using AiTrainer.Web.Persistence.EntityFramework.Repositories.Abstract;
using AiTrainer.Web.Persistence.Models;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Concrete;

namespace AiTrainer.Web.Domain.Services.Common.Workflow.Activities
{
    internal class SaveModelToDbActivity<TModel, TEnt, TEntId> : BaseActivity<IReadOnlyCollection<TModel>, DbSaveResult<TModel>>
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
    {
        public override string Description => "This activity is intended to save a entity to the database";
        private readonly IRepository<TEnt, TEntId, TModel> _repo;
        public SaveModelToDbActivity(IRepository<TEnt, TEntId, TModel> repo)
        {
            _repo = repo;
        }
        public override async Task<(ActivityResultEnum ActivityResult, DbSaveResult<TModel>? ActualResult)> ExecuteAsync(IReadOnlyCollection<TModel>? workflowContextItem)
        {
            try
            {

                var dtosToCreate = new List<TModel>();
                var dtosToUpdate= new List<TModel>();

                foreach(var allEnts in workflowContextItem)
                {
                    var foundId = DomainModelExtensions.GetPropertyValue<object>(allEnts,"Id".ToPascalCase());

                    if(foundId is null)
                    {
                        dtosToCreate.Add(allEnts);
                    }
                    else
                    {
                        dtosToUpdate.Add(allEnts);
                    }
                }


                return (ActivityResultEnum.Fail, null);

            }
            catch (Exception e)
            {
                return (ActivityResultEnum.Fail, null);
            }
        }
    }
}
