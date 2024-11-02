using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Persistence.EntityFramework.Entities;
using AiTrainer.Web.Persistence.EntityFramework.Repositories.Abstract;
using AiTrainer.Web.Persistence.Models;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Concrete;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.Workflow.Activities
{
    internal class SaveModelToDbActivity<TModel, TEnt, TEntId> : BaseActivity<IReadOnlyCollection<TModel>, DbSaveResult<TModel>>
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
    {
        public override string Description => "This activity is intended to save a entity to the database";
        private readonly IRepository<TEnt, TEntId, TModel> _repo;
        private readonly ILogger<SaveModelToDbActivity<TModel, TEnt, TEntId>> _logger;
        public SaveModelToDbActivity(IRepository<TEnt, TEntId, TModel> repo, ILogger<SaveModelToDbActivity<TModel, TEnt, TEntId>> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        public override async Task<(ActivityResultEnum ActivityResult, DbSaveResult<TModel>? ActualResult)> ExecuteAsync(IReadOnlyCollection<TModel>? workflowContextItem)
        {
            try
            {

                var dtosToCreate = new List<TModel>();
                var dtosToUpdate = new List<TModel>();

                foreach (var allEnts in workflowContextItem ?? [])
                {
                    var foundId = DomainModelExtensions.GetPropertyValue<object>(allEnts, "Id".ToPascalCase());

                    if (foundId is null)
                    {
                        dtosToCreate.Add(allEnts);
                    }
                    else
                    {
                        dtosToUpdate.Add(allEnts);
                    }
                }

                if (dtosToCreate.Count > 0 || dtosToUpdate.Count > 0)
                {
                    if (dtosToCreate.Count == 0)
                    {
                        var newUpdatedList = await _repo.Update(dtosToUpdate);
                        return (ActivityResultEnum.Success, newUpdatedList);
                    }
                    else if (dtosToUpdate.Count == 0)
                    {
                        var newCreatedList = await _repo.Create(dtosToCreate);
                        return (ActivityResultEnum.Success, newCreatedList);
                    }
                    else
                    {
                        var newSavedList = await Task.WhenAll(_repo.Update(dtosToUpdate), _repo.Create(dtosToCreate));

                        return (ActivityResultEnum.Success, new DbSaveResult<TModel>(newSavedList.SelectMany(x => x.Data!).ToArray()));
                    }
                }


                return (ActivityResultEnum.Fail, null);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Saving to db finished with exception message {Message}", e.Message);

                return (ActivityResultEnum.Fail, null);
            }
        }
    }
}
