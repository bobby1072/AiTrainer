namespace AiTrainer.Web.Workflow.Contexts
{
    public abstract class BaseWorkflowContext
    {
        public abstract BaseWorkflowInputContext Input { get; }
        public abstract BaseWorkflowOutputContext Output { get; }
    }
}
