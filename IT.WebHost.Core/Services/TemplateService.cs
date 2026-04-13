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
            { BaseFiles.Wildfire, "wildfire" },
            { BaseFiles.CyberPunkGemma, "cyberpunk-gemma" },
            { BaseFiles.CyberPunkGemma2, "cyberpunk-gemma2" },
            { BaseFiles.CyberPunkGemma3, "cyberpunk-gemma3" },
            { BaseFiles.CyberPunkQwen, "cyberpunk-qwen" },
        };
        // todo: load this from settings service
        public string TemplateFile { get; set; } = "test";
    }

    public enum BaseFiles
    {
        Default,
        Papyrus,
        Aurora,
        Galaxy,
        Wildfire,
        Reef,
        CyberPunkGemma,
        CyberPunkGemma2,
        CyberPunkGemma3,
        CyberPunkQwen,
    }
}
