using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POE2Tools.Utilities
{
    public class PlayerStatus
    {
        private Main _main;
        private WindowsUtil _windowsUtil;
        private InputHook _inputHook;
        private ColorUtil _colorUtil;


        private int _colorTolerance = 10;
        private Color HIGH_POISON_SAMPLE_1 = Color.FromArgb(255, 110, 133, 85);
        private Color HIGH_POISON_SAMPLE_2 = Color.FromArgb(255, 56, 68, 38);
        private Color LOW_POISON_SAMPLE_1 = Color.FromArgb(255, 81, 107, 46);
        private Color LOW_POISON_SAMPLE_2 = Color.FromArgb(255, 47, 64, 21);

        private bool _isHighLife = false;
        private bool _isLowLife = false;
        private bool _isHighShield = false;
        private bool _isLowShield = false;
        private bool _isHighMana = false;
        private bool _isLowMana = false;
        private bool _isPoisoned = false;

        private bool _sampled = false;


        // LIFE =======================================================================
        private const float HIGH_LIFE_PIXEL_RATIO_X_1 = 0.049f;
        private const float HIGH_LIFE_PIXEL_RATIO_X_2 = 0.092f;
        private const float HIGH_LIFE_PIXEL_RATIO_Y = 0.85f;
        private const float LOW_LIFE_PIXEL_RATIO_X_1 = 0.036f;
        private const float LOW_LIFE_PIXEL_RATIO_X_2 = 0.1f;
        private const float LOW_LIFE_PIXEL_RATIO_Y = 0.89f;

        private Point _highLifePoint1 = new Point();
        private Point _highLifePoint2 = new Point();
        private Point _lowLifePoint1 = new Point();
        private Point _lowLifePoint2 = new Point();
        private Color _highLifeSample1 = Color.White;
        private Color _highLifeSample2 = Color.White;
        private Color _lowLifeSample1 = Color.White;
        private Color _lowLifeSample2 = Color.White;

        private bool _needLifeCheck = false;
        // ============================================================================



        // SHIELD =====================================================================
        private const float HIGH_SHIELD_PIXEL_RATIO_X = 0.105f;
        private const float HIGH_SHIELD_PIXEL_RATIO_Y = 0.845f;
        private const float LOW_SHIELD_PIXEL_RATIO_X = 0.112f;
        private const float LOW_SHIELD_PIXEL_RATIO_Y = 0.942f;

        private Point _highShieldPoint = new Point();
        private Point _lowShieldPoint = new Point();
        private Color _highShieldSample = Color.White;
        private Color _lowShieldSample = Color.White;

        private bool _needShieldCheck = false;
        // ============================================================================



        // MANA =======================================================================
        private const float HIGH_MANA_PIXEL_RATIO_X = 0.968f;
        private const float HIGH_MANA_PIXEL_RATIO_Y = 0.865f;
        private const float LOW_MANA_PIXEL_RATIO_X = 0.96f;
        private const float LOW_MANA_PIXEL_RATIO_Y = 0.945f;

        private Point _highManaPoint = new Point();
        private Color _highManaSample = Color.White;
        private Point _lowManaPoint = new Point();
        private Color _lowManaSample = Color.White;

        private bool _needManaCheck = false;
        // ============================================================================




        public PlayerStatus(Main main, WindowsUtil windowsUtil, InputHook inputHook, ColorUtil colorUtil)
        {
            _main = main;
            _windowsUtil = windowsUtil;
            _inputHook = inputHook;
            _colorUtil = colorUtil;

            _highLifePoint1 = _colorUtil.GetPixelPosition(HIGH_LIFE_PIXEL_RATIO_X_1, HIGH_LIFE_PIXEL_RATIO_Y);
            _highLifePoint2 = _colorUtil.GetPixelPosition(HIGH_LIFE_PIXEL_RATIO_X_2, HIGH_LIFE_PIXEL_RATIO_Y);
            _lowLifePoint1 = _colorUtil.GetPixelPosition(LOW_LIFE_PIXEL_RATIO_X_1, LOW_LIFE_PIXEL_RATIO_Y);
            _lowLifePoint2 = _colorUtil.GetPixelPosition(LOW_LIFE_PIXEL_RATIO_X_2, LOW_LIFE_PIXEL_RATIO_Y);

            _highShieldPoint = _colorUtil.GetPixelPosition(HIGH_SHIELD_PIXEL_RATIO_X, HIGH_SHIELD_PIXEL_RATIO_Y);
            _lowShieldPoint = _colorUtil.GetPixelPosition(LOW_SHIELD_PIXEL_RATIO_X, LOW_SHIELD_PIXEL_RATIO_Y);

            _highManaPoint = _colorUtil.GetPixelPosition(HIGH_MANA_PIXEL_RATIO_X, HIGH_MANA_PIXEL_RATIO_Y);
            _lowManaPoint = _colorUtil.GetPixelPosition(LOW_MANA_PIXEL_RATIO_X, LOW_MANA_PIXEL_RATIO_Y);
        }

        public void RegisterLifeCheck(bool need)
        {
            _needLifeCheck = need;
        }

        public void RegisterShieldCheck(bool need)
        {
            _needShieldCheck = need;
        }

        public void RegisterManaCheck(bool need)
        {
            _needManaCheck = need;
        }

        public void Sample()
        {
            _highLifeSample1 = _colorUtil.GetColorAt(_highLifePoint1);
            _highLifeSample2 = _colorUtil.GetColorAt(_highLifePoint2);
            _lowLifeSample1 = _colorUtil.GetColorAt(_lowLifePoint1);
            _lowLifeSample2 = _colorUtil.GetColorAt(_lowLifePoint2);
            _highShieldSample = _colorUtil.GetColorAt(_highShieldPoint);
            _lowShieldSample = _colorUtil.GetColorAt(_lowShieldPoint);
            _highManaSample = _colorUtil.GetColorAt(_highManaPoint);
            _lowManaSample = _colorUtil.GetColorAt(_lowManaPoint);

            _main.pnlHighLifeSample.BackColor = _highLifeSample1;
            _main.pnlLowLifeSample.BackColor = _lowLifeSample1;
            _main.pnlHighShieldSample.BackColor = _highShieldSample;
            _main.pnlLowShieldSample.BackColor = _lowShieldSample;
            _main.pnlHighManaSample.BackColor = _highManaSample;
            _main.pnlLowManaSample.BackColor = _lowManaSample;

            _sampled = true;
        }

        public void AddDebugPoint()
        {
            _windowsUtil.AddDebugDrawPoint(_highLifePoint1);
            _windowsUtil.AddDebugDrawPoint(_highLifePoint2);
            _windowsUtil.AddDebugDrawPoint(_lowLifePoint1);
            _windowsUtil.AddDebugDrawPoint(_lowLifePoint2);
            _windowsUtil.AddDebugDrawPoint(_highShieldPoint);
            _windowsUtil.AddDebugDrawPoint(_lowShieldPoint);
            _windowsUtil.AddDebugDrawPoint(_highManaPoint);
            _windowsUtil.AddDebugDrawPoint(_lowManaPoint);
        }

        public void RemoveDebugPoint()
        {
            _windowsUtil.RemoveDebugDrawPoint(_highLifePoint1);
            _windowsUtil.RemoveDebugDrawPoint(_highLifePoint2);
            _windowsUtil.RemoveDebugDrawPoint(_lowLifePoint1);
            _windowsUtil.RemoveDebugDrawPoint(_lowLifePoint2);
            _windowsUtil.RemoveDebugDrawPoint(_highShieldPoint);
            _windowsUtil.RemoveDebugDrawPoint(_lowShieldPoint);
            _windowsUtil.RemoveDebugDrawPoint(_highManaPoint);
            _windowsUtil.RemoveDebugDrawPoint(_lowManaPoint);
        }

        public void SetColorTolerance(int tolerance)
        {
            _colorTolerance = tolerance;
        }

        public void Start()
        {
            if (!_sampled)
            {
                Sample();
            }
        }

        public void Stop()
        {

        }

        public void MainLoop(int deltaTime, bool shouldDoLogic, bool started)
        {
            if (started && shouldDoLogic)
            {
                Color highLifeColor1 = Color.Black;
                Color highLifeColor2 = Color.Black;
                Color lowLifeColor1 = Color.Black;
                Color lowLifeColor2 = Color.Black;
                Color highShieldColor = Color.Black;
                Color lowShieldColor = Color.Black;
                Color highManaColor = Color.Black;
                Color lowManaColor = Color.Black;

                
                // Life
                if (_main.IsDebugMode() || _needLifeCheck)
                {
                    // Sample current value
                    highLifeColor1 = _colorUtil.GetColorAt(_highLifePoint1);
                    highLifeColor2 = _colorUtil.GetColorAt(_highLifePoint2);
                    lowLifeColor1 = _colorUtil.GetColorAt(_lowLifePoint1);
                    lowLifeColor2 = _colorUtil.GetColorAt(_lowLifePoint2);

                    // Check high life
                    if (_colorUtil.IsColorSimilar(highLifeColor1, HIGH_POISON_SAMPLE_1, _colorTolerance) && _colorUtil.IsColorSimilar(highLifeColor2, HIGH_POISON_SAMPLE_2, _colorTolerance))
                    {
                        _isHighLife = false;
                    }
                    else if (_colorUtil.IsColorSimilar(highLifeColor1, _highLifeSample1, _colorTolerance) || _colorUtil.IsColorSimilar(highLifeColor2, _highLifeSample2, _colorTolerance))
                    {
                        _isHighLife = false;
                    }
                    else
                    {
                        _isHighLife = true;
                    }

                    // Check low life
                    if (_colorUtil.IsColorSimilar(lowLifeColor1, LOW_POISON_SAMPLE_1, _colorTolerance) && _colorUtil.IsColorSimilar(lowLifeColor2, LOW_POISON_SAMPLE_2, _colorTolerance))
                    {
                        _isPoisoned = true;
                        _isLowLife = false;
                    }
                    else if (_colorUtil.IsColorSimilar(lowLifeColor1, _lowLifeSample1, _colorTolerance  ) || _colorUtil.IsColorSimilar(lowLifeColor2, _lowLifeSample2, _colorTolerance))
                    {
                        _isPoisoned = false;
                        _isLowLife = false;
                    }
                    else
                    {
                        _isPoisoned = false;
                        _isLowLife = true;
                    }
                }




                // Shield
                if (_main.IsDebugMode() || _needShieldCheck)
                {
                    highShieldColor = _colorUtil.GetColorAt(_highShieldPoint);
                    lowShieldColor = _colorUtil.GetColorAt(_lowShieldPoint);

                    // Check high shield
                    if (_colorUtil.IsColorSimilar(highShieldColor, _highShieldSample, _colorTolerance))
                    {
                        _isHighShield = false;
                    }
                    else
                    {
                        _isHighShield = true;
                    }

                    // Check low shield
                    if (_colorUtil.IsColorSimilar(lowShieldColor, _lowShieldSample, _colorTolerance))
                    {
                        _isLowShield = false;
                    }
                    else
                    {
                        _isLowShield = true;
                    }
                }





                // Mana
                if (_main.IsDebugMode() || _needManaCheck)
                {
                    highManaColor = _colorUtil.GetColorAt(_highManaPoint);
                    lowManaColor = _colorUtil.GetColorAt(_lowManaPoint);

                    // Check high mana
                    if (_colorUtil.IsColorSimilar(highManaColor, _highManaSample, _colorTolerance))
                    {
                        _isHighMana = false;
                    }
                    else
                    {
                        _isHighMana = true;
                    }

                    // Check low mana
                    if (_colorUtil.IsColorSimilar(lowManaColor, _lowManaSample, _colorTolerance))
                    {
                        _isLowMana = false;
                    }
                    else
                    {
                        _isLowMana = true;
                    }
                }

                

                

                


                // Debug
                if (_main.IsDebugMode())
                {
                    _main.pnlHighLifeActual.BackColor = highLifeColor1;
                    _main.pnlLowLifeActual.BackColor = lowLifeColor1;
                    _main.pnlHighShieldActual.BackColor = highShieldColor;
                    _main.pnlLowShieldActual.BackColor = lowShieldColor;
                    _main.pnlHighManaActual.BackColor = highManaColor;
                    _main.pnlLowManaActual.BackColor = lowManaColor;

                    _main.chkHighLifeDebug.Checked = _isHighLife;
                    _main.chkLowLifeDebug.Checked = _isLowLife;
                    _main.chkHighShieldDebug.Checked = _isHighShield;
                    _main.chkLowShieldDebug.Checked = _isLowShield;
                    _main.chkHighManaDebug.Checked = _isHighMana;
                    _main.chkLowManaDebug.Checked = _isLowMana;
                    _main.chkPoisonDebug.Checked = _isPoisoned;
                }
            }
        }

        public bool IsHighLife()
        {
            return _isHighLife;
        }

        public bool IsLowLife()
        {
            return _isLowLife;
        }

        public bool IsHighShield()
        {
            return _isHighShield;
        }

        public bool IsLowShield()
        {
            return _isLowShield;
        }

        public bool IsHighMana()
        {
            return _isHighMana;
        }

        public bool IsLowMana()
        {
            return _isLowMana;
        }

        public bool IsPoisoned()
        {
            return _isPoisoned;
        }
    }
}
