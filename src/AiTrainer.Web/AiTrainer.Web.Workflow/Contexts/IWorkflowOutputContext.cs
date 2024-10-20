namespace AiTrainer.Web.Workflow.Contexts
{
    public interface IWorkflowOutputContext<TReturn>
    {
        public abstract TReturn? ReturnObject { get; set; }
    }
}
