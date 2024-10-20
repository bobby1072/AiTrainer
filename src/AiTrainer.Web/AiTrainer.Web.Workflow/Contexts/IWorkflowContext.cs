namespace AiTrainer.Web.Workflow.Contexts
{
    public interface IWorkflowContext<TInputContext, TOutputContext, TReturn>
        where TInputContext : IWorkflowInputContext
        where TOutputContext : IWorkflowOutputContext<TReturn>
    {
        public TInputContext Input { get; }
        public TOutputContext Output { get; }
    }
}
