
namespace AiTrainer.Web.Common.Models.Configuration
{
    public class ApplicationSettingsConfiguration: BaseConfiguration
    {
        public static readonly string Key = GetKey(nameof(ApplicationSettingsConfiguration));
        public required string Name { get; init; }   
        public required string ReleaseVersion { get; init; }
    }
}
