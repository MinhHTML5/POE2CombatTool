using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POE2Tools.Utilities;

namespace POE2Tools.Modules
{
    public class SprintModule
    {
        public const int DODGE_PROCESS_IDLE = 0;
        public const int DODGE_PROCESS_WAIT_RELEASE = 1;

        public const float SPACEBAR_RELEASE_TIME = 250;
        public const float AUTO_RELOAD_TIME = 500;

        public Main _main;
        public WindowsUtil _windowsUtil;
        public InputHook _inputHook;
        public PlayerStatus _playerStatus;

        private bool _responsiveDodge = false;
        private int _dodgeProcessStep = DODGE_PROCESS_IDLE;
        private float _dodgeProcessCount = 0;
        private bool _spaceBarHolding = false;
        private bool _shiftHolding = false;
        private bool _skipNextSpaceBar = false;


        private bool _autoReload = false;
        private float _autoReloadCount = 0;

        public SprintModule(Main main, WindowsUtil windowsUtil, InputHook inputHook, PlayerStatus playerStatus)
        {
            _main = main;
            _windowsUtil = windowsUtil;
            _inputHook = inputHook;
            _playerStatus = playerStatus;
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void MainLoop(int deltaTime, bool shouldDoLogic, bool started)
        {
            if (_responsiveDodge && started && shouldDoLogic)
            {
                if (_dodgeProcessStep == DODGE_PROCESS_WAIT_RELEASE)
                {
                    if (_shiftHolding)
                    {
                        _dodgeProcessStep = DODGE_PROCESS_IDLE;
                        _dodgeProcessCount = 0;
                        if (_spaceBarHolding)
                        {

                        }
                        return;
                    }
                    _dodgeProcessCount += deltaTime;
                    if (_dodgeProcessCount >= SPACEBAR_RELEASE_TIME)
                    {
                        _skipNextSpaceBar = true;
                        _inputHook.SendKeyUp(Keys.Space);
                        _dodgeProcessStep = DODGE_PROCESS_IDLE;
                        _dodgeProcessCount = 0;
                    }
                }
            }

            if (_autoReload && started && shouldDoLogic && _autoReloadCount < AUTO_RELOAD_TIME)
            {
                _autoReloadCount += deltaTime;
                if (_autoReloadCount >= AUTO_RELOAD_TIME)
                {
                    _inputHook.SendKeyDown(Keys.F);
                    _inputHook.SendKeyUp(Keys.F);
                }
            }
        }

        public void SetResponsiveDodge(bool value)
        {
            _responsiveDodge = value;
        }

        public void SetAutoReload(bool value)
        {
            _autoReload = value;
        }

        public void SpaceEventDetected(bool isDown)
        {
            if (_skipNextSpaceBar)
            {
                _skipNextSpaceBar = false;
                return;
            }

            _spaceBarHolding = isDown;
            if (_responsiveDodge == true && isDown)
            {
                if (_dodgeProcessStep == DODGE_PROCESS_IDLE)
                {
                    _dodgeProcessCount = 0;
                    _dodgeProcessStep = DODGE_PROCESS_WAIT_RELEASE;

                    _autoReloadCount = 0;
                }
            }
            if (_main.IsDebugMode())
            {
                _main.chkSpaceStatus.Checked = isDown;
            }
        }

        public void ShiftEventDetected(bool isDown)
        {
            _shiftHolding = isDown;
            if (_responsiveDodge == true && isDown)
            {
                if (_spaceBarHolding && _dodgeProcessStep == DODGE_PROCESS_IDLE)
                {
                    _skipNextSpaceBar = true;
                    _inputHook.SendKeyDown(Keys.Space);
                }
            }

            if (_main.IsDebugMode())
            {
                _main.chkShiftHold.Checked = isDown;
            }
        }
    }
}
