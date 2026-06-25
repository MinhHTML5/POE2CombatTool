using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using POE2RuleTool.Models;
using POE2Tools.Utilities;

namespace POE2RuleTool.Services;

public sealed class RuleRunner
{
    private const uint MouseLeftDown = 0x0002;
    private const uint MouseLeftUp = 0x0004;
    private const uint MouseRightDown = 0x0008;
    private const uint MouseRightUp = 0x0010;
    private const uint MouseMiddleDown = 0x0020;
    private const uint MouseMiddleUp = 0x0040;
    private const uint MouseXDown = 0x0080;
    private const uint MouseXUp = 0x0100;
    private const uint XButton1 = 0x0001;
    private const uint XButton2 = 0x0002;

    private readonly ColorUtil _colorUtil;
    private readonly InputHook _inputHook;
    private readonly Dictionary<Guid, RuleRuntimeState> _states = new();

    public RuleRunner(ColorUtil colorUtil, InputHook inputHook)
    {
        _colorUtil = colorUtil;
        _inputHook = inputHook;
    }

    public bool IsRunning { get; private set; }
    public event Action<string>? StatusChanged;

    public void Start(IEnumerable<RuleDefinition> rules)
    {
        EnsureStates(rules);
        IsRunning = true;
        StatusChanged?.Invoke("Rule runner started.");
    }

    public void Stop()
    {
        IsRunning = false;
        StatusChanged?.Invoke("Rule runner stopped.");
    }

    public void Tick(IEnumerable<RuleDefinition> rules)
    {
        if (!IsRunning)
        {
            return;
        }

        EnsureStates(rules);
        long now = Environment.TickCount64;

        foreach (RuleDefinition rule in rules)
        {
            RuleRuntimeState state = _states[rule.Id];
            if (!rule.Enabled || state.IsExecuting || now < state.NextCheckAt)
            {
                continue;
            }

            if (EvaluateRule(rule))
            {
                state.NextCheckAt = now + Math.Max(0, rule.CooldownMs);
                state.IsExecuting = true;
                _ = ExecuteRuleAsync(rule, state);
            }
        }
    }

    private bool EvaluateRule(RuleDefinition rule)
    {
        if (rule.Conditions.Count == 0 || rule.Actions.Count == 0)
        {
            return false;
        }

        int changedCount = 0;
        foreach (PixelCondition condition in rule.Conditions)
        {
            Color currentColor = _colorUtil.GetColorAt(condition.Point);
            bool changed = condition.HasChangedFromSample(currentColor);

            if (changed)
            {
                changedCount++;
            }
            else if (rule.ConditionMode == ConditionJoinMode.And)
            {
                return false;
            }
        }

        return rule.ConditionMode switch
        {
            ConditionJoinMode.And => changedCount == rule.Conditions.Count,
            ConditionJoinMode.Or => changedCount > 0,
            ConditionJoinMode.Xor => changedCount == 1,
            _ => false
        };
    }

    private async Task ExecuteRuleAsync(RuleDefinition rule, RuleRuntimeState state)
    {
        try
        {
            foreach (RuleActionDefinition action in rule.Actions)
            {
                ExecuteAction(action);

                if (action.DelayAfterMs > 0)
                {
                    await Task.Delay(action.DelayAfterMs).ConfigureAwait(false);
                }
            }

            StatusChanged?.Invoke($"Rule triggered: {rule.Name}");
        }
        catch (Exception ex)
        {
            StatusChanged?.Invoke($"Rule failed: {rule.Name}: {ex.Message}");
        }
        finally
        {
            state.IsExecuting = false;
        }
    }

    private void ExecuteAction(RuleActionDefinition action)
    {
        if (action.Kind == RuleActionKind.KeyboardKeyPress)
        {
            if (action.Key != Keys.None)
            {
                _inputHook.PressKey(action.Key, action.ControlModifier);
            }

            return;
        }

        if (action.MouseButton == MouseButtons.Left)
        {
            _inputHook.SendLeftClick(action.ControlModifier);
            return;
        }

        if (action.ControlModifier)
        {
            _inputHook.SendKeyDown(Keys.ControlKey);
        }

        try
        {
            SendMouseClick(action.MouseButton);
        }
        finally
        {
            if (action.ControlModifier)
            {
                _inputHook.SendKeyUp(Keys.ControlKey);
            }
        }
    }

    private void SendMouseClick(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.Left:
                _inputHook.SendLeftClick();
                break;
            case MouseButtons.Right:
                MouseClick(MouseRightDown, MouseRightUp, 0);
                break;
            case MouseButtons.Middle:
                MouseClick(MouseMiddleDown, MouseMiddleUp, 0);
                break;
            case MouseButtons.XButton1:
                MouseClick(MouseXDown, MouseXUp, XButton1);
                break;
            case MouseButtons.XButton2:
                MouseClick(MouseXDown, MouseXUp, XButton2);
                break;
        }
    }

    private static void MouseClick(uint down, uint up, uint data)
    {
        mouse_event(down, 0, 0, data, UIntPtr.Zero);
        mouse_event(up, 0, 0, data, UIntPtr.Zero);
    }

    private void EnsureStates(IEnumerable<RuleDefinition> rules)
    {
        HashSet<Guid> activeIds = new();
        foreach (RuleDefinition rule in rules)
        {
            activeIds.Add(rule.Id);
            if (!_states.ContainsKey(rule.Id))
            {
                _states[rule.Id] = new RuleRuntimeState();
            }
        }

        foreach (Guid staleId in _states.Keys.Where(id => !activeIds.Contains(id)).ToList())
        {
            _states.Remove(staleId);
        }
    }

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    private sealed class RuleRuntimeState
    {
        public bool IsExecuting { get; set; }
        public long NextCheckAt { get; set; }
    }
}

