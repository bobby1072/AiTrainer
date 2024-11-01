using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Attributes;
using BT.Common.WorkflowActivities.Activities.Concrete;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AiTrainer.Web.Domain.Services.Common.Workflow.Activities
{
    internal class ValidateDomainModelActivity<TModelToValidate>: ValidateModelActivity<TModelToValidate> where TModelToValidate : DomainModel<object> 
    {
        public override string Description => "This activity validates a domain model and returns a failed activity result if the validation fails";

        public ValidateDomainModelActivity(IValidator<TModelToValidate> validator, ILogger<ValidateDomainModelActivity<TModelToValidate>> logger): base(validator, logger)
        {}

        public override async Task<(ActivityResultEnum ActivityResult, ValidationResult? ActualResult)> ExecuteAsync(TModelToValidate? workflowContextItem)
        {
            var finalValidationResult = ActivityResultEnum.Success;
            var (baseActivityResult, validationResult) = await base.ExecuteAsync(workflowContextItem);
            finalValidationResult = baseActivityResult;
            
            if (workflowContextItem?.Id is not null)
            {
                var modelType = workflowContextItem.GetType();
                if (modelType.GetCustomAttribute<LockedPropertyAttribute>() is not null)
                {

                }
            }

            return (finalValidationResult, validationResult);
        }
    }
}
