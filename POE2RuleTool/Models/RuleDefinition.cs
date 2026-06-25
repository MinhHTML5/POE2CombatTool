namespace POE2RuleTool.Models;

public sealed class RuleDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "New rule";
    public bool Enabled { get; set; } = true;
    public ConditionJoinMode ConditionMode { get; set; } = ConditionJoinMode.And;
    public int CooldownMs { get; set; } = 1000;
    public List<PixelCondition> Conditions { get; set; } = new();
    public List<RuleActionDefinition> Actions { get; set; } = new();

    public RuleDefinition Clone()
    {
        return new RuleDefinition
        {
            Id = Id,
            Name = Name,
            Enabled = Enabled,
            ConditionMode = ConditionMode,
            CooldownMs = CooldownMs,
            Conditions = Conditions.Select(condition => condition.Clone()).ToList(),
            Actions = Actions.Select(action => action.Clone()).ToList()
        };
    }

    public int SampledConditionCount => Conditions.Count(condition => condition.HasSample);
}
