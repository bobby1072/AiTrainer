namespace AiTrainer.Web.Common.Exceptions
{
    public class AiTrainerException : Exception
    {
        public AiTrainerException(string message)
            : base(message) { }

        public AiTrainerException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
