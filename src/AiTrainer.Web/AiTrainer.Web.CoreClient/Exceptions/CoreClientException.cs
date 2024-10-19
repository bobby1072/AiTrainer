using AiTrainer.Web.Common.Exceptions;

namespace AiTrainer.Web.CoreClient.Exceptions
{
    public class CoreClientException : AiTrainerException
    {
        public CoreClientException(string message)
            : base(message) { }
    }
}
