namespace POE2RuleTool.Models;

public sealed class RuleToolConfig
{
    public List<RuleDefinition> Rules { get; set; } = new();
    public CustomModuleConfig Modules { get; set; } = new();
}

public sealed class CustomModuleConfig
{
    public bool SmartSprintEnabled { get; set; }
}
