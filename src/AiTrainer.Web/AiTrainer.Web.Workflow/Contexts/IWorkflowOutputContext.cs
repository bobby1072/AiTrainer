namespace AiTrainer.Web.Workflow.Contexts
{
    public interface IWorkflowOutputContext<TReturn>
    {
        TReturn? ReturnObject { get; set; }
    }
}
