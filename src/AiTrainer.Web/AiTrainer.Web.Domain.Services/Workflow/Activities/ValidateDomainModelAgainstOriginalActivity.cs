using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Extensions;
using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Concrete;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.Workflow.Activities
{
    internal class ValidateDomainModelAgainstOriginalActivity<TModelToValidate, TModelId>
        : BaseActivity<(TModelToValidate, TModelToValidate?), bool>
        where TModelToValidate : DomainModel<TModelId>
    {
        public override string Description =>
            "This activity validates a domain model against the original domain model returns a failed activity result if locked values are changed";
        private readonly ILogger<
            ValidateDomainModelAgainstOriginalActivity<TModelToValidate, TModelId>
        > _logger;

        public ValidateDomainModelAgainstOriginalActivity(
            ILogger<ValidateDomainModelAgainstOriginalActivity<TModelToValidate, TModelId>> logger
        )
        {
            _logger = logger;
        }

        public override Task<(ActivityResultEnum ActivityResult, bool ActualResult)> ExecuteAsync(
            (TModelToValidate, TModelToValidate?) workflowContextItem
        )
        {
            var (newModel, originalModel) = workflowContextItem;

            if (originalModel is null)
            {
                return Task.FromResult((ActivityResultEnum.Skip, true));
            }

            var isNewModelOk = newModel.ValidateAgainstOriginal<TModelToValidate, TModelId>(
                originalModel
            );

            return Task.FromResult(
                (isNewModelOk ? ActivityResultEnum.Success : ActivityResultEnum.Fail, isNewModelOk)
            );
        }
    }
}
