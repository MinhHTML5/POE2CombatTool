using System;
using System.Drawing;
using System.Windows.Forms;
using POE2Tools.Modules;
using POE2Tools.Utilities;
using System.Diagnostics;

namespace POE2Tools
{
    public partial class Main : Form
    {
        public static Main sInstance;

        public const int UPDATE_INTERVAL = 40;
        public const int COLOR_TOLERANCE = 5;

        private WindowsUtil _windowsUtil;
        private InputHook _inputHook;
        private ColorUtil _colorUtil;

        private PlayerStatus _playerStatus;
        private SkillModule _skillModule;
        private SprintModule _sprintModule;

        private bool _started = false;
        private bool _debug = true;

        private Timer _timer = new Timer();
        private Stopwatch _stopwatch = new Stopwatch();


        public Main(WindowsUtil windowsUtil, InputHook inputHook, ColorUtil colorUtil)
        {
            sInstance = this;

            _windowsUtil = windowsUtil;
            _inputHook = inputHook;
            _colorUtil = colorUtil;

            InitializeComponent();
        }

        private const int WM_INPUT = 0x00FF;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_INPUT)
            {
                _inputHook.ProcessRawInput(m.LParam);
            }

            base.WndProc(ref m);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _playerStatus = new PlayerStatus(this, _windowsUtil, _inputHook, _colorUtil);
            _skillModule = new SkillModule(this, _windowsUtil, _inputHook, _playerStatus);
            _sprintModule = new SprintModule(this, _windowsUtil, _inputHook, _playerStatus);

            _inputHook.RegisterRawInputDevices(this.Handle, OnMouseKeyEvent, OnKeyEvent);

            _timer.Interval = UPDATE_INTERVAL;
            _timer.Tick += (s, e) => MainLoop();
            _timer.Start();
            _stopwatch.Start();

            LoadSettings();

            _playerStatus.AddDebugPoint();
        }

        private void Start()
        {
            SaveSettings();
            btnStartStop.Text = "STOP";
            _playerStatus.Start();
            _skillModule.Start();
            _sprintModule.Start();
            _windowsUtil.SetStarted(true);
        }

        private void Stop()
        {
            btnStartStop.Text = "START";
            _playerStatus.Stop();
            _skillModule.Stop();
            _sprintModule.Stop();
            _windowsUtil.SetStarted(false);
        }

        private void MainLoop()
        {
            // This variable turn off all submodule from doing logic, but still let them to count cooldown
            bool shouldDoLogic = true;
            int deltaTime = (int)(_stopwatch.Elapsed.TotalMilliseconds);
            lblDeltaTime.Text = deltaTime.ToString();
            _stopwatch.Restart();

            // Check for game focus
            if (_windowsUtil.GetCurrentWindowsProcessName() != "PathOfExile" && _started)
            {
                shouldDoLogic = false;
                lblMessage.Text = "POE is out of focus. Current window: " + _windowsUtil.GetCurrentWindowsProcessName();
            }

            // Check for loading screen
            if (shouldDoLogic && _started)
            {
                Color cornerColor1 = _colorUtil.GetColorAt(new Point(5, Screen.PrimaryScreen.Bounds.Height - 4));
                Color cornerColor2 = _colorUtil.GetColorAt(new Point(Screen.PrimaryScreen.Bounds.Width - 4, Screen.PrimaryScreen.Bounds.Height - 4));
                if (_colorUtil.IsColorSimilar(cornerColor1, Color.Black, COLOR_TOLERANCE)
                && _colorUtil.IsColorSimilar(cornerColor2, Color.Black, COLOR_TOLERANCE))
                {
                    shouldDoLogic = false;
                    lblMessage.Text = "POE is in loading screen. (Probably)";
                }
            }

            if (!_started)
            {
                lblMessage.Text = "Toolbox is not started...";
            }
            else if (shouldDoLogic)
            {
                lblMessage.Text = "Toolbox is working...";
            }

            _playerStatus.MainLoop(deltaTime, shouldDoLogic, _started);
            _skillModule.MainLoop(deltaTime, shouldDoLogic, _started);
            _sprintModule.MainLoop(deltaTime, shouldDoLogic, _started);
        }

        public bool IsDebugMode()
        {
            return _debug;
        }



        private void OnKeyEvent(Keys key, bool isDown, bool isControlDown)
        {
            if ((key == Keys.B) && !isDown && isControlDown)
            {
                _started = !_started;
                if (_started)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
            else if (key == Keys.Space)
            {
                _sprintModule.SpaceEventDetected(isDown);
            }
            else if (key == Keys.ShiftKey)
            {
                _sprintModule.ShiftEventDetected(isDown);
            }
        }

        private void OnMouseKeyEvent(MouseButtons key, bool isDown)
        {
            if (key == MouseButtons.XButton2)
            {
                _sprintModule.MouseForwardEventDetected(isDown);
            }
        }

        private void lblSample_Click(object sender, EventArgs e)
        {
            _playerStatus.Sample();
        }
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            _started = !_started;
            if (_started)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        private void chkHighLife_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            int index = 0;
            if (checkBox == chkFlaskHighLife1) index = 0;
            else if (checkBox == chkFlaskHighLife2) index = 1;
            else if (checkBox == chkSkillHighLife1) index = 2;
            else if (checkBox == chkSkillHighLife2) index = 3;
            else if (checkBox == chkSkillHighLife3) index = 4;
            else if (checkBox == chkSkillHighLife4) index = 5;
            else if (checkBox == chkSkillHighLife5) index = 6;
            _skillModule.SetUseSkillHighLife(index, checkBox.Checked);
        }

        private void chkLowLife_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            int index = 0;
            if (checkBox == chkFlaskLowLife1) index = 0;
            else if (checkBox == chkFlaskLowLife2) index = 1;
            else if (checkBox == chkSkillLowLife1) index = 2;
            else if (checkBox == chkSkillLowLife2) index = 3;
            else if (checkBox == chkSkillLowLife3) index = 4;
            else if (checkBox == chkSkillLowLife4) index = 5;
            else if (checkBox == chkSkillLowLife5) index = 6;
            _skillModule.SetUseSkillLowLife(index, checkBox.Checked);
        }

        private void chkHighShield_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            int index = 0;
            if (checkBox == chkFlaskHighShield1) index = 0;
            else if (checkBox == chkFlaskHighShield2) index = 1;
            else if (checkBox == chkSkillHighShield1) index = 2;
            else if (checkBox == chkSkillHighShield2) index = 3;
            else if (checkBox == chkSkillHighShield3) index = 4;
            else if (checkBox == chkSkillHighShield4) index = 5;
            else if (checkBox == chkSkillHighShield5) index = 6;
            _skillModule.SetUseSkillHighShield(index, checkBox.Checked);
        }

        private void chkLowShield_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            int index = 0;
            if (checkBox == chkFlaskLowShield1) index = 0;
            else if (checkBox == chkFlaskLowShield2) index = 1;
            else if (checkBox == chkSkillLowShield1) index = 2;
            else if (checkBox == chkSkillLowShield2) index = 3;
            else if (checkBox == chkSkillLowShield3) index = 4;
            else if (checkBox == chkSkillLowShield4) index = 5;
            else if (checkBox == chkSkillLowShield5) index = 6;
            _skillModule.SetUseSkillLowShield(index, checkBox.Checked);
        }

        private void chkHighMana_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            int index = 0;
            if (checkBox == chkFlaskHighMana1) index = 0;
            else if (checkBox == chkFlaskHighMana2) index = 1;
            else if (checkBox == chkSkillHighMana1) index = 2;
            else if (checkBox == chkSkillHighMana2) index = 3;
            else if (checkBox == chkSkillHighMana3) index = 4;
            else if (checkBox == chkSkillHighMana4) index = 5;
            else if (checkBox == chkSkillHighMana5) index = 6;
            _skillModule.SetUseSkillHighMana(index, checkBox.Checked);
        }

        private void chkLowMana_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            int index = 0;
            if (checkBox == chkFlaskLowMana1) index = 0;
            else if (checkBox == chkFlaskLowMana2) index = 1;
            else if (checkBox == chkSkillLowMana1) index = 2;
            else if (checkBox == chkSkillLowMana2) index = 3;
            else if (checkBox == chkSkillLowMana3) index = 4;
            else if (checkBox == chkSkillLowMana4) index = 5;
            else if (checkBox == chkSkillLowMana5) index = 6;
            _skillModule.SetUseSkillLowMana(index, checkBox.Checked);
        }

        private void chkCooldown_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            int index = 0;
            if (checkBox == chkFlaskCooldown1) index = 0;
            else if (checkBox == chkFlaskCooldown2) index = 1;
            else if (checkBox == chkSkillCooldown1) index = 2;
            else if (checkBox == chkSkillCooldown2) index = 3;
            else if (checkBox == chkSkillCooldown3) index = 4;
            else if (checkBox == chkSkillCooldown4) index = 5;
            else if (checkBox == chkSkillCooldown5) index = 6;
            _skillModule.SetUseSkillLatency(index, checkBox.Checked);
        }

        private void txtSkillCooldown_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            int index = 0;
            if (textBox == txtFlaskCooldown1) index = 0;
            else if (textBox == txtFlaskCooldown2) index = 1;
            else if (textBox == txtSkillCooldown1) index = 2;
            else if (textBox == txtSkillCooldown2) index = 3;
            else if (textBox == txtSkillCooldown3) index = 4;
            else if (textBox == txtSkillCooldown4) index = 5;
            else if (textBox == txtSkillCooldown5) index = 6;

            if (!int.TryParse(textBox.Text, out int textValue))
            {
                textBox.BackColor = Color.LightCoral;
            }
            else
            {
                textBox.BackColor = SystemColors.Window;
                _skillModule.SetSkillCooldown(index, textValue);
            }
        }

        private void chkDebugMode_CheckedChanged(object sender, EventArgs e)
        {
            _debug = chkDebugMode.Checked;
            _windowsUtil.SetDebugEnabled(_debug);
        }

        private void chkDrawText_CheckedChanged(object sender, EventArgs e)
        {
            _windowsUtil.SetDrawTextOn(chkDrawText.Checked);
        }

        private void chkSmartSprint_CheckedChanged(object sender, EventArgs e)
        {
            _sprintModule.SetResponsiveDodge(chkSmartSprint.Checked);
        }


        private void UpdateRateChange()
        {
            int value = trkUpdateRate.Value * 5;
            lblUpdateRate.Text = value.ToString() + "ms";
            _timer.Interval = value;
        }
        private void trkUpdateRate_Scroll(object sender, EventArgs e)
        {
            UpdateRateChange();
        }

        private void UpdateColorTolerance()
        {
            int value = trkColorTolerance.Value * 2;
            lblColorTolerance.Text = value.ToString();
        }
        private void trkColorTolerance_Scroll(object sender, EventArgs e)
        {
            UpdateColorTolerance();
        }


        private void SaveSettings()
        {
            Properties.Settings.Default.trkUpdateRate = trkUpdateRate.Value;
            Properties.Settings.Default.trkColorTolerance = trkColorTolerance.Value;
            Properties.Settings.Default.chkDebugMode = chkDebugMode.Checked;
            Properties.Settings.Default.chkDrawText = chkDrawText.Checked;

            Properties.Settings.Default.txtFlaskCooldown1 = txtFlaskCooldown1.Text;
            Properties.Settings.Default.chkFlaskHighLife1 = chkFlaskHighLife1.Checked;
            Properties.Settings.Default.chkFlaskLowLife1 = chkFlaskLowLife1.Checked;
            Properties.Settings.Default.chkFlaskHighShield1 = chkFlaskHighShield1.Checked;
            Properties.Settings.Default.chkFlaskLowShield1 = chkFlaskLowShield1.Checked;
            Properties.Settings.Default.chkFlaskHighMana1 = chkFlaskHighMana1.Checked;
            Properties.Settings.Default.chkFlaskLowMana1 = chkFlaskLowMana1.Checked;
            Properties.Settings.Default.chkFlaskCooldown1 = chkFlaskCooldown1.Checked;

            Properties.Settings.Default.txtFlaskCooldown2 = txtFlaskCooldown2.Text;
            Properties.Settings.Default.chkFlaskHighLife2 = chkFlaskHighLife2.Checked;
            Properties.Settings.Default.chkFlaskLowLife2 = chkFlaskLowLife2.Checked;
            Properties.Settings.Default.chkFlaskHighShield2 = chkFlaskHighShield2.Checked;
            Properties.Settings.Default.chkFlaskLowShield2 = chkFlaskLowShield2.Checked;
            Properties.Settings.Default.chkFlaskHighMana2 = chkFlaskHighMana2.Checked;
            Properties.Settings.Default.chkFlaskLowMana2 = chkFlaskLowMana2.Checked;
            Properties.Settings.Default.chkFlaskCooldown2 = chkFlaskCooldown2.Checked;

            Properties.Settings.Default.txtSkillCooldown1 = txtSkillCooldown1.Text;
            Properties.Settings.Default.chkSkillHighLife1 = chkSkillHighLife1.Checked;
            Properties.Settings.Default.chkSkillLowLife1 = chkSkillLowLife1.Checked;
            Properties.Settings.Default.chkSkillHighShield1 = chkSkillHighShield1.Checked;
            Properties.Settings.Default.chkSkillLowShield1 = chkSkillLowShield1.Checked;
            Properties.Settings.Default.chkSkillHighMana1 = chkSkillHighMana1.Checked;
            Properties.Settings.Default.chkSkillLowMana1 = chkSkillLowMana1.Checked;
            Properties.Settings.Default.chkSkillCooldown1 = chkSkillCooldown1.Checked;

            Properties.Settings.Default.txtSkillCooldown2 = txtSkillCooldown2.Text;
            Properties.Settings.Default.chkSkillHighLife2 = chkSkillHighLife2.Checked;
            Properties.Settings.Default.chkSkillLowLife2 = chkSkillLowLife2.Checked;
            Properties.Settings.Default.chkSkillHighShield2 = chkSkillHighShield2.Checked;
            Properties.Settings.Default.chkSkillLowShield2 = chkSkillLowShield2.Checked;
            Properties.Settings.Default.chkSkillHighMana2 = chkSkillHighMana2.Checked;
            Properties.Settings.Default.chkSkillLowMana2 = chkSkillLowMana2.Checked;
            Properties.Settings.Default.chkSkillCooldown2 = chkSkillCooldown2.Checked;

            Properties.Settings.Default.txtSkillCooldown3 = txtSkillCooldown3.Text;
            Properties.Settings.Default.chkSkillHighLife3 = chkSkillHighLife3.Checked;
            Properties.Settings.Default.chkSkillLowLife3 = chkSkillLowLife3.Checked;
            Properties.Settings.Default.chkSkillHighShield3 = chkSkillHighShield3.Checked;
            Properties.Settings.Default.chkSkillLowShield3 = chkSkillLowShield3.Checked;
            Properties.Settings.Default.chkSkillHighMana3 = chkSkillHighMana3.Checked;
            Properties.Settings.Default.chkSkillLowMana3 = chkSkillLowMana3.Checked;
            Properties.Settings.Default.chkSkillCooldown3 = chkSkillCooldown3.Checked;

            Properties.Settings.Default.txtSkillCooldown4 = txtSkillCooldown4.Text;
            Properties.Settings.Default.chkSkillHighLife4 = chkSkillHighLife4.Checked;
            Properties.Settings.Default.chkSkillLowLife4 = chkSkillLowLife4.Checked;
            Properties.Settings.Default.chkSkillHighShield4 = chkSkillHighShield4.Checked;
            Properties.Settings.Default.chkSkillLowShield4 = chkSkillLowShield4.Checked;
            Properties.Settings.Default.chkSkillHighMana4 = chkSkillHighMana4.Checked;
            Properties.Settings.Default.chkSkillLowMana4 = chkSkillLowMana4.Checked;
            Properties.Settings.Default.chkSkillCooldown4 = chkSkillCooldown4.Checked;

            Properties.Settings.Default.txtSkillCooldown5 = txtSkillCooldown5.Text;
            Properties.Settings.Default.chkSkillHighLife5 = chkSkillHighLife5.Checked;
            Properties.Settings.Default.chkSkillLowLife5 = chkSkillLowLife5.Checked;
            Properties.Settings.Default.chkSkillHighShield5 = chkSkillHighShield5.Checked;
            Properties.Settings.Default.chkSkillLowShield5 = chkSkillLowShield5.Checked;
            Properties.Settings.Default.chkSkillHighMana5 = chkSkillHighMana5.Checked;
            Properties.Settings.Default.chkSkillLowMana5 = chkSkillLowMana5.Checked;
            Properties.Settings.Default.chkSkillCooldown5 = chkSkillCooldown5.Checked;

            Properties.Settings.Default.chkSmartSprint = chkSmartSprint.Checked;

            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            chkDebugMode.Checked = Properties.Settings.Default.chkDebugMode;
            chkDrawText.Checked = Properties.Settings.Default.chkDrawText;
            trkUpdateRate.Value = Properties.Settings.Default.trkUpdateRate;
            trkColorTolerance.Value = Properties.Settings.Default.trkColorTolerance;

            txtFlaskCooldown1.Text = Properties.Settings.Default.txtFlaskCooldown1;
            chkFlaskHighLife1.Checked = Properties.Settings.Default.chkFlaskHighLife1;
            chkFlaskLowLife1.Checked = Properties.Settings.Default.chkFlaskLowLife1;
            chkFlaskHighShield1.Checked = Properties.Settings.Default.chkFlaskHighShield1;
            chkFlaskLowShield1.Checked = Properties.Settings.Default.chkFlaskLowShield1;
            chkFlaskHighMana1.Checked = Properties.Settings.Default.chkFlaskHighMana1;
            chkFlaskLowMana1.Checked = Properties.Settings.Default.chkFlaskLowMana1;
            chkFlaskCooldown1.Checked = Properties.Settings.Default.chkFlaskCooldown1;

            txtFlaskCooldown2.Text = Properties.Settings.Default.txtFlaskCooldown2;
            chkFlaskHighLife2.Checked = Properties.Settings.Default.chkFlaskHighLife2;
            chkFlaskLowLife2.Checked = Properties.Settings.Default.chkFlaskLowLife2;
            chkFlaskHighShield2.Checked = Properties.Settings.Default.chkFlaskHighShield2;
            chkFlaskLowShield2.Checked = Properties.Settings.Default.chkFlaskLowShield2;
            chkFlaskHighMana2.Checked = Properties.Settings.Default.chkFlaskHighMana2;
            chkFlaskLowMana2.Checked = Properties.Settings.Default.chkFlaskLowMana2;
            chkFlaskCooldown2.Checked = Properties.Settings.Default.chkFlaskCooldown2;

            txtSkillCooldown1.Text = Properties.Settings.Default.txtSkillCooldown1;
            chkSkillHighLife1.Checked = Properties.Settings.Default.chkSkillHighLife1;
            chkSkillLowLife1.Checked = Properties.Settings.Default.chkSkillLowLife1;
            chkSkillHighShield1.Checked = Properties.Settings.Default.chkSkillHighShield1;
            chkSkillLowShield1.Checked = Properties.Settings.Default.chkSkillLowShield1;
            chkSkillHighMana1.Checked = Properties.Settings.Default.chkSkillHighMana1;
            chkSkillLowMana1.Checked = Properties.Settings.Default.chkSkillLowMana1;
            chkSkillCooldown1.Checked = Properties.Settings.Default.chkSkillCooldown1;

            txtSkillCooldown2.Text = Properties.Settings.Default.txtSkillCooldown2;
            chkSkillHighLife2.Checked = Properties.Settings.Default.chkSkillHighLife2;
            chkSkillLowLife2.Checked = Properties.Settings.Default.chkSkillLowLife2;
            chkSkillHighShield2.Checked = Properties.Settings.Default.chkSkillHighShield2;
            chkSkillLowShield2.Checked = Properties.Settings.Default.chkSkillLowShield2;
            chkSkillHighMana2.Checked = Properties.Settings.Default.chkSkillHighMana2;
            chkSkillLowMana2.Checked = Properties.Settings.Default.chkSkillLowMana2;
            chkSkillCooldown2.Checked = Properties.Settings.Default.chkSkillCooldown2;

            txtSkillCooldown3.Text = Properties.Settings.Default.txtSkillCooldown3;
            chkSkillHighLife3.Checked = Properties.Settings.Default.chkSkillHighLife3;
            chkSkillLowLife3.Checked = Properties.Settings.Default.chkSkillLowLife3;
            chkSkillHighShield3.Checked = Properties.Settings.Default.chkSkillHighShield3;
            chkSkillLowShield3.Checked = Properties.Settings.Default.chkSkillLowShield3;
            chkSkillHighMana3.Checked = Properties.Settings.Default.chkSkillHighMana3;
            chkSkillLowMana3.Checked = Properties.Settings.Default.chkSkillLowMana3;
            chkSkillCooldown3.Checked = Properties.Settings.Default.chkSkillCooldown3;

            txtSkillCooldown4.Text = Properties.Settings.Default.txtSkillCooldown4;
            chkSkillHighLife4.Checked = Properties.Settings.Default.chkSkillHighLife4;
            chkSkillLowLife4.Checked = Properties.Settings.Default.chkSkillLowLife4;
            chkSkillHighShield4.Checked = Properties.Settings.Default.chkSkillHighShield4;
            chkSkillLowShield4.Checked = Properties.Settings.Default.chkSkillLowShield4;
            chkSkillHighMana4.Checked = Properties.Settings.Default.chkSkillHighMana4;
            chkSkillLowMana4.Checked = Properties.Settings.Default.chkSkillLowMana4;
            chkSkillCooldown4.Checked = Properties.Settings.Default.chkSkillCooldown4;

            txtSkillCooldown5.Text = Properties.Settings.Default.txtSkillCooldown5;
            chkSkillHighLife5.Checked = Properties.Settings.Default.chkSkillHighLife5;
            chkSkillLowLife5.Checked = Properties.Settings.Default.chkSkillLowLife5;
            chkSkillHighShield5.Checked = Properties.Settings.Default.chkSkillHighShield5;
            chkSkillLowShield5.Checked = Properties.Settings.Default.chkSkillLowShield5;
            chkSkillHighMana5.Checked = Properties.Settings.Default.chkSkillHighMana5;
            chkSkillLowMana5.Checked = Properties.Settings.Default.chkSkillLowMana5;
            chkSkillCooldown5.Checked = Properties.Settings.Default.chkSkillCooldown5;

            chkSmartSprint.Checked = Properties.Settings.Default.chkSmartSprint;

            UpdateRateChange();
        }


        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }
    }
}
