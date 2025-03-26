namespace AiTrainer.Web.Common.Configuration
{
    public abstract class BaseConfiguration
    {
        protected static string GetKey(string nameOfClass) => nameOfClass.Replace("Configuration", "");
    }
}
