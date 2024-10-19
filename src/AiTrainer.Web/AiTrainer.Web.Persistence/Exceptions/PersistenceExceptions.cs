using AiTrainer.Web.Common.Exceptions;

namespace AiTrainer.Web.Persistence.Exceptions
{
    public class PersistenceExceptions : AiTrainerException
    {
        public PersistenceExceptions(string message)
            : base(message) { }
    }
}
