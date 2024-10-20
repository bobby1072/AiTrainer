using AiTrainer.Web.Workflow.Models;

namespace AiTrainer.Web.Workflow.Activities.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RetryActivity : Attribute
    {
        public ActivityResultEnum ResultToRetryOn { get; init; }
        public int NumberOfRetries { get; init; }
        public int SecondsBetweenRetries { get; init; }

        public RetryActivity(
            ActivityResultEnum resultToRetryOn,
            int numberOfRetries,
            int secondsBetweenRetries
        )
        {
            ResultToRetryOn = resultToRetryOn;
            NumberOfRetries = numberOfRetries;
            SecondsBetweenRetries = secondsBetweenRetries;
        }
    }
}
