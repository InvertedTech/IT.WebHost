namespace IT.WebHost.Core.Services
{
    public class TemplateService
    {
        public readonly Dictionary<BaseFiles, string> BaseTemplates = new Dictionary<BaseFiles, string>()
        {
            { BaseFiles.Default, "default" },
            { BaseFiles.Papyrus, "papyrus" },
            { BaseFiles.Aurora, "aurora" },
            { BaseFiles.Galaxy, "galaxy" },
            { BaseFiles.Reef, "reef" },
            { BaseFiles.Wildfire, "wildfire" }
        };
        // todo: load this from settings service
        public string TemplateFile { get; set; } = "reef";
    }

    public enum BaseFiles
    {
        Default,
        Papyrus,
        Aurora,
        Galaxy,
        Wildfire,
        Reef
    }
}
