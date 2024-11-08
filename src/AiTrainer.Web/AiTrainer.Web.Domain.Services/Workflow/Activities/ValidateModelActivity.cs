using BT.Common.WorkflowActivities.Activities.Abstract;
using BT.Common.WorkflowActivities.Activities.Concrete;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.Workflow.Activities
{
    internal class ValidateModelActivity<TModelToValidate>
        : BaseActivity<TModelToValidate, ValidationResult>
    {
        public override string Description =>
            "This activity validates a model and returns a failed activity result if the validation fails";
        private readonly IValidator<TModelToValidate> _validator;
        private readonly ILogger<ValidateModelActivity<TModelToValidate>> _logger;

        public ValidateModelActivity(
            IValidator<TModelToValidate> validator,
            ILogger<ValidateModelActivity<TModelToValidate>> logger
        )
        {
            _validator = validator;
            _logger = logger;
        }

        public override async Task<(
            ActivityResultEnum ActivityResult,
            ValidationResult? ActualResult
        )> ExecuteAsync(TModelToValidate? workflowContextItem)
        {
            if (workflowContextItem is null)
            {
                return (ActivityResultEnum.Skip, null);
            }

            var validationResults = await _validator.ValidateAsync(workflowContextItem);

            if (!validationResults.IsValid)
            {
                _logger.LogDebug(
                    "Validated object of type {TypeName} was invalid",
                    workflowContextItem.GetType().Name
                );
            }

            return (
                validationResults.IsValid ? ActivityResultEnum.Success : ActivityResultEnum.Fail,
                validationResults
            );
        }
    }
}
