using System.Windows.Forms;

namespace POE2RuleTool.Models;

public sealed class RuleActionDefinition
{
    public RuleActionKind Kind { get; set; } = RuleActionKind.KeyboardKeyPress;
    public Keys Key { get; set; } = Keys.None;
    public MouseButtons MouseButton { get; set; } = MouseButtons.Left;
    public bool ControlModifier { get; set; }
    public int DelayAfterMs { get; set; }

    public RuleActionDefinition Clone()
    {
        return new RuleActionDefinition
        {
            Kind = Kind,
            Key = Key,
            MouseButton = MouseButton,
            ControlModifier = ControlModifier,
            DelayAfterMs = DelayAfterMs
        };
    }

    public string DescribeInput()
    {
        return Kind == RuleActionKind.KeyboardKeyPress
            ? $"{(ControlModifier ? "Ctrl + " : string.Empty)}{Key}"
            : $"{(ControlModifier ? "Ctrl + " : string.Empty)}{MouseButton}";
    }
}
