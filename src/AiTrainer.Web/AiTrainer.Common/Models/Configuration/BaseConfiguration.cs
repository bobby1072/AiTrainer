
namespace AiTrainer.Web.Common.Models.Configuration
{
    public abstract class BaseConfiguration
    {
        protected static string GetKey(string nameOfClass) => nameOfClass.Replace("Configuration", "");
    }
}
