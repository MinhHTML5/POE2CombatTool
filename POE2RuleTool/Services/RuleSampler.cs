using POE2RuleTool.Models;
using POE2Tools.Utilities;

namespace POE2RuleTool.Services;

public static class RuleSampler
{
    public static int SampleRules(IEnumerable<RuleDefinition> rules, ColorUtil colorUtil)
    {
        int sampled = 0;

        foreach (RuleDefinition rule in rules)
        {
            foreach (PixelCondition condition in rule.Conditions)
            {
                condition.SampleColor = colorUtil.GetColorAt(condition.Point);
                sampled++;
            }
        }

        return sampled;
    }

    public static int SampleMissingRules(IEnumerable<RuleDefinition> rules, ColorUtil colorUtil)
    {
        int sampled = 0;

        foreach (RuleDefinition rule in rules)
        {
            foreach (PixelCondition condition in rule.Conditions.Where(condition => !condition.HasSample))
            {
                condition.SampleColor = colorUtil.GetColorAt(condition.Point);
                sampled++;
            }
        }

        return sampled;
    }
}
