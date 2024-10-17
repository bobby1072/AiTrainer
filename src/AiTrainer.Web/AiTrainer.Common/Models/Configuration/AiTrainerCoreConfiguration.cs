namespace AiTrainer.Web.Common.Models.Configuration
{
    public class AiTrainerCoreConfiguration: BaseConfiguration
    {
        public static readonly string Key = GetKey(nameof(AiTrainerCoreConfiguration)); 
        public string AiTrainerCore { get; init; }
        public string BaseRoute { get; init; }
    }
}
