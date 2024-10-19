using AiTrainer.Web.Workflow.Activities;

namespace AiTrainer.Web.Workflow.Models
{
    public record CompletedActivityResult
    {
        public IActivity ActualActivity { get; init; }
        public ActivityResultEnum ActivityResultEnum { get; init; }
        public DateTime HappenAt { get; init; }
        public TimeSpan TimeTaken { get; init; }
    }
}
