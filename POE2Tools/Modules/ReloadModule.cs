using POE2Tools.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POE2Tools.Modules
{
    public class ReloadModule
    {
        public const float AUTO_RELOAD_TIME = 500;

        public Main _main;
        public WindowsUtil _windowsUtil;
        public InputHook _inputHook;
        public PlayerStatus _playerStatus;

        private bool _autoReload = false;
        private float _autoReloadLeftCount = 0;
        private float _autoReloadRightCount = 0;

        public ReloadModule(Main main, WindowsUtil windowsUtil, InputHook inputHook, PlayerStatus playerStatus)
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
            if (_autoReload && started && shouldDoLogic && _autoReloadLeftCount < AUTO_RELOAD_TIME)
            {
                _autoReloadLeftCount += deltaTime;
                if (_autoReloadLeftCount >= AUTO_RELOAD_TIME)
                {
                    _inputHook.SendKeyDown(Keys.T);
                    _inputHook.SendKeyUp(Keys.T);
                }
            }

            if (_autoReload && started && shouldDoLogic && _autoReloadRightCount < AUTO_RELOAD_TIME)
            {
                _autoReloadRightCount += deltaTime;
                if (_autoReloadRightCount >= AUTO_RELOAD_TIME)
                {
                    _inputHook.SendKeyDown(Keys.F);
                    _inputHook.SendKeyUp(Keys.F);
                }
            }
        }

        public void SetAutoReload(bool value)
        {
            _autoReload = value;
        }

        public void LeftClick()
        {
            if (_autoReload)
            {
                _autoReloadLeftCount = 0;
            }
        }

        public void RightClick()
        {
            if (_autoReload)
            {
                _autoReloadRightCount = 0;
            }
        }
    }
}
