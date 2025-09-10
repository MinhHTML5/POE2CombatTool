using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POE2Tools.Utilities;

namespace POE2Tools.Modules
{
    public class SkillModule
    {
        public const int SKILL_NUMBER = 7;

        public Main _main;
        public WindowsUtil _windowsUtil;
        public InputHook _inputHook;
        public PlayerStatus _playerStatus;

        private List<bool> _useSkillHighLifeIndexArray = new List<bool>();
        private List<bool> _useSkillLowLifeIndexArray = new List<bool>();
        private List<bool> _useSkillHighShieldIndexArray = new List<bool>();
        private List<bool> _useSkillLowShieldIndexArray = new List<bool>();
        private List<bool> _useSkillHighManaIndexArray = new List<bool>();
        private List<bool> _useSkillLowManaIndexArray = new List<bool>();
        private List<bool> _useSkillLatencyIndexArray = new List<bool>();
        private List<int> _useSkillCooldownArray = new List<int>();

        private List<int> _skillCooldownCountArray = new List<int>();

        public SkillModule(Main main, WindowsUtil windowsUtil, InputHook inputHook, PlayerStatus playerStatus)
        {
            _main = main;
            _windowsUtil = windowsUtil;
            _inputHook = inputHook;
            _playerStatus = playerStatus;

            for (int i = 0; i<SKILL_NUMBER; i++)
            {
                _useSkillHighLifeIndexArray.Add(false);
                _useSkillLowLifeIndexArray.Add(false);
                _useSkillHighShieldIndexArray.Add(false);
                _useSkillLowShieldIndexArray.Add(false);
                _useSkillHighManaIndexArray.Add(false);
                _useSkillLowManaIndexArray.Add(false);
                _useSkillLatencyIndexArray.Add(false);
                _useSkillCooldownArray.Add(4000);
                _skillCooldownCountArray.Add(0);
            }
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void SetUseSkillHighLife(int index, bool value)
        {
            if (index < SKILL_NUMBER)
            {
                _useSkillHighLifeIndexArray[index] = value;
            }
            CheckRegister();
        }
        public void SetUseSkillLowLife(int index, bool value)
        {
            if (index < SKILL_NUMBER)
            {
                _useSkillLowLifeIndexArray[index] = value;
            }
            CheckRegister();
        }
        public void SetUseSkillHighShield(int index, bool value)
        {
            if (index < SKILL_NUMBER)
            {
                _useSkillHighShieldIndexArray[index] = value;
            }
            CheckRegister();
        }
        public void SetUseSkillLowShield(int index, bool value)
        {
            if (index < SKILL_NUMBER)
            {
                _useSkillLowShieldIndexArray[index] = value;
            }
            CheckRegister();
        }
        public void SetUseSkillHighMana(int index, bool value)
        {
            if (index < SKILL_NUMBER)
            {
                _useSkillHighManaIndexArray[index] = value;
            }
            CheckRegister();
        }
        public void SetUseSkillLowMana(int index, bool value)
        {
            if (index < SKILL_NUMBER)
            {
                _useSkillLowManaIndexArray[index] = value;
            }
            CheckRegister();
        }
        public void SetUseSkillLatency(int index, bool value)
        {
            if (index < SKILL_NUMBER)
            {
                _useSkillLatencyIndexArray[index] = value;
            }
        }

        public void SetSkillCooldown(int index, int value)
        {
            if (index < SKILL_NUMBER)
            {
                _useSkillCooldownArray[index] = value;
            }
        }

        public void CheckRegister()
        {
            bool needLifeCheck = false;
            bool needShieldCheck = false;
            bool needManaCheck = false;

            for (int i = 0; i < SKILL_NUMBER; i++)
            {
                if (_useSkillHighLifeIndexArray[i]) needLifeCheck = true;
                if (_useSkillLowLifeIndexArray[i]) needLifeCheck = true;
                if (_useSkillHighShieldIndexArray[i]) needShieldCheck = true;
                if (_useSkillLowShieldIndexArray[i]) needShieldCheck = true;
                if (_useSkillHighManaIndexArray[i]) needManaCheck = true;
                if (_useSkillLowManaIndexArray[i]) needManaCheck = true;
            }

            _playerStatus.RegisterLifeCheck(needLifeCheck);
            _playerStatus.RegisterShieldCheck(needShieldCheck);
            _playerStatus.RegisterManaCheck(needManaCheck);
        }


        public void MainLoop(int deltaTime, bool shouldDoLogic, bool started)
        {
            for (int i = 0; i < SKILL_NUMBER; i++)
            {
                _skillCooldownCountArray[i] -= deltaTime;
                if (_skillCooldownCountArray[i] < 0) _skillCooldownCountArray[i] = 0;

                if (_skillCooldownCountArray[i] <= 0 && started && shouldDoLogic)
                {
                    if (_useSkillHighLifeIndexArray[i] && _playerStatus.IsHighLife())
                    {
                        UseSkill(i);
                    }
                    else if (_useSkillLowLifeIndexArray[i] && _playerStatus.IsLowLife())
                    {
                        UseSkill(i);
                    }
                    else if (_useSkillHighShieldIndexArray[i] && _playerStatus.IsHighShield())
                    {
                        UseSkill(i);
                    }
                    else if (_useSkillLowShieldIndexArray[i] && _playerStatus.IsLowShield())
                    {
                        UseSkill(i);
                    }
                    else if (_useSkillHighManaIndexArray[i] && _playerStatus.IsHighMana())
                    {
                        UseSkill(i);
                    }
                    else if (_useSkillLowManaIndexArray[i] && _playerStatus.IsLowMana())
                    {
                        UseSkill(i);
                    }
                    else if (_useSkillLatencyIndexArray[i] == true)
                    {
                        UseSkill(i);
                    }
                }
            }
        }

        public void UseSkill(int index)
        {
            _skillCooldownCountArray[index] = _useSkillCooldownArray[index];
            switch (index)
            {
                case 0:
                    _inputHook.PressKey(Keys.D1, false);
                    break;
                case 1:
                    _inputHook.PressKey(Keys.D2, false);
                    break;
                case 2:
                    _inputHook.PressKey(Keys.Q, true);
                    break;
                case 3:
                    _inputHook.PressKey(Keys.E, true);
                    break;
                case 4:
                    _inputHook.PressKey(Keys.R, true);
                    break;
                case 5:
                    _inputHook.PressKey(Keys.T, true);
                    break;
                case 6:
                    _inputHook.PressKey(Keys.F, true);
                    break;
            }
        }

    }
}
