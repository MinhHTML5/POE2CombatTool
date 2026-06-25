using System.Windows.Forms;
using POE2Tools.Utilities;

namespace POE2RuleTool.Modules;

public sealed class SprintModule
{
    private const int DodgeProcessIdle = 0;
    private const int DodgeProcessWaitRelease = 1;
    private const float SpacebarReleaseTime = 250;

    private readonly InputHook _inputHook;

    private bool _responsiveDodge;
    private bool _started;
    private int _dodgeProcessStep = DodgeProcessIdle;
    private float _dodgeProcessCount;
    private bool _spaceBarHolding;
    private bool _shiftHolding;
    private bool _skipNextSpaceBar;

    public SprintModule(InputHook inputHook)
    {
        _inputHook = inputHook;
    }

    public void Start()
    {
        _started = true;
    }

    public void Stop()
    {
        _started = false;
        _dodgeProcessStep = DodgeProcessIdle;
        _dodgeProcessCount = 0;
        _skipNextSpaceBar = false;
    }

    public void MainLoop(int deltaTime, bool shouldDoLogic, bool started)
    {
        if (!_responsiveDodge || !started || !_started || !shouldDoLogic)
        {
            return;
        }

        if (_dodgeProcessStep != DodgeProcessWaitRelease)
        {
            return;
        }

        if (_shiftHolding)
        {
            _dodgeProcessStep = DodgeProcessIdle;
            _dodgeProcessCount = 0;
            return;
        }

        _dodgeProcessCount += deltaTime;
        if (_dodgeProcessCount >= SpacebarReleaseTime)
        {
            _skipNextSpaceBar = true;
            _inputHook.SendKeyUp(Keys.Space);
            _dodgeProcessStep = DodgeProcessIdle;
            _dodgeProcessCount = 0;
        }
    }

    public void SetResponsiveDodge(bool value)
    {
        _responsiveDodge = value;
        if (!value)
        {
            _dodgeProcessStep = DodgeProcessIdle;
            _dodgeProcessCount = 0;
            _skipNextSpaceBar = false;
        }
    }

    public void SpaceEventDetected(bool isDown)
    {
        if (_skipNextSpaceBar)
        {
            _skipNextSpaceBar = false;
            return;
        }

        _spaceBarHolding = isDown;
        if (_responsiveDodge && _started && isDown && _dodgeProcessStep == DodgeProcessIdle)
        {
            _dodgeProcessCount = 0;
            _dodgeProcessStep = DodgeProcessWaitRelease;
        }
    }

    public void ShiftEventDetected(bool isDown)
    {
        _shiftHolding = isDown;
        if (_responsiveDodge && _started && isDown && _spaceBarHolding && _dodgeProcessStep == DodgeProcessIdle)
        {
            _skipNextSpaceBar = true;
            _inputHook.SendKeyDown(Keys.Space);
        }
    }
}
