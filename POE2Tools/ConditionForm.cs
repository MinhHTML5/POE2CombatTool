using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using POE2Tools.Modules;
using POE2Tools.Utilities;

namespace POE2Tools
{
    // Edits one condition of an action preset step
    public partial class ConditionForm : Form
    {
        // Positions in cboTarget, the steps come after those
        private const int TARGET_INDEX_NEXT = 0;
        private const int TARGET_INDEX_END = 1;
        private const int TARGET_INDEX_FIRST_STEP = 2;

        private AutomationModule _automationModule;
        private List<string> _stepNames;
        private Bitmap _sample;

        // The edited condition. Read it back when the dialog returns OK
        public ActionCondition Condition { get; private set; }

        public ConditionForm(AutomationModule automationModule, ActionCondition condition, List<string> stepNames)
        {
            _automationModule = automationModule;
            Condition = condition;
            _stepNames = stepNames;
            InitializeComponent();
        }

        private void ConditionForm_Load(object sender, EventArgs e)
        {
            cboTarget.Items.Add("The next step");
            cboTarget.Items.Add("Nowhere, end the entire action");
            for (int i = 0; i < _stepNames.Count; i++)
            {
                cboTarget.Items.Add("Step " + (i + 1) + ": " + _stepNames[i]);
            }

            // The items of cboType are in the same order as ConditionType
            cboType.SelectedIndex = (int)Condition.Type;
            numInterval.Value = Math.Max(numInterval.Minimum, Math.Min(numInterval.Maximum, Condition.CheckInterval));
            numTolerance.Value = Math.Max(numTolerance.Minimum, Math.Min(numTolerance.Maximum, Condition.ColorTolerance));
            numPercent.Value = Math.Max(numPercent.Minimum, Math.Min(numPercent.Maximum, Condition.MatchPercent));

            if (Condition.Target == ConditionTarget.EndAction) cboTarget.SelectedIndex = TARGET_INDEX_END;
            else if (Condition.Target == ConditionTarget.GoToStep && Condition.TargetStep >= 0 && Condition.TargetStep < _stepNames.Count)
                cboTarget.SelectedIndex = TARGET_INDEX_FIRST_STEP + Condition.TargetStep;
            else cboTarget.SelectedIndex = TARGET_INDEX_NEXT;

            _sample = ScreenMatcher.FromBase64(Condition.SampleBase64);
            UpdateSample();
        }

        private void ConditionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            picSample.Image = null;
            _sample?.Dispose();
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConditionType type = (ConditionType)cboType.SelectedIndex;
            grpScreen.Enabled = type == ConditionType.ScreenMatch;

            // The number box is the loop count or the seconds, depending on the type
            bool hasValue = type == ConditionType.MacroLooped || type == ConditionType.MacroRanFor;
            lblValue.Visible = hasValue;
            numValue.Visible = hasValue;
            lblValueUnit.Visible = hasValue;
            if (type == ConditionType.MacroLooped)
            {
                lblValue.Text = "Number of loops:";
                lblValueUnit.Text = "times";
                numValue.DecimalPlaces = 0;
                numValue.Minimum = 1;
                numValue.Value = Math.Max(numValue.Minimum, Math.Min(numValue.Maximum, Condition.LoopCount));
            }
            else if (type == ConditionType.MacroRanFor)
            {
                lblValue.Text = "Running time:";
                lblValueUnit.Text = "seconds (checked at the end of each loop)";
                numValue.DecimalPlaces = 1;
                numValue.Minimum = 0.1m;
                numValue.Value = Math.Max(numValue.Minimum, Math.Min(numValue.Maximum, (decimal)Condition.RunSeconds));
            }
        }

        private void UpdateSample()
        {
            picSample.Image = _sample;
            lblRegion.Text = _sample == null
                ? "No sample yet"
                : "At (" + Condition.RegionX + ", " + Condition.RegionY + "), " + _sample.Width + " x " + _sample.Height + " pixels";
            btnTest.Enabled = _sample != null;
            lblTestResult.Text = "";
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            using (CaptureForm captureForm = new CaptureForm(_automationModule))
            {
                if (captureForm.ShowDialog(this) != DialogResult.OK) return;

                picSample.Image = null;
                _sample?.Dispose();
                _sample = captureForm.Sample;
                Condition.RegionX = captureForm.SampleRegion.X;
                Condition.RegionY = captureForm.SampleRegion.Y;
                Condition.RegionWidth = captureForm.SampleRegion.Width;
                Condition.RegionHeight = captureForm.SampleRegion.Height;
                Condition.SampleBase64 = ScreenMatcher.ToBase64(_sample);
                UpdateSample();
            }
        }

        // Compare the sample with what is on the screen right now, to help picking the numbers
        private void btnTest_Click(object sender, EventArgs e)
        {
            if (_sample == null) return;
            try
            {
                using (ScreenMatcher matcher = new ScreenMatcher(_sample, Condition.Region))
                {
                    double percent = matcher.GetMatchPercent((int)numTolerance.Value);
                    bool matches = percent >= (double)numPercent.Value;
                    lblTestResult.ForeColor = matches ? Color.Green : Color.Red;
                    lblTestResult.Text = percent.ToString("0.0") + "% of the pixels match";
                }
            }
            catch (Exception ex)
            {
                lblTestResult.ForeColor = Color.Red;
                lblTestResult.Text = ex.Message;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ConditionType type = (ConditionType)cboType.SelectedIndex;
            bool isScreen = type == ConditionType.ScreenMatch;
            if (isScreen && _sample == null)
            {
                MessageBox.Show(this, "Capture a sample first.", "Condition", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Condition.Type = type;
            if (type == ConditionType.MacroLooped) Condition.LoopCount = (int)numValue.Value;
            if (type == ConditionType.MacroRanFor) Condition.RunSeconds = (double)numValue.Value;
            Condition.CheckInterval = (int)numInterval.Value;
            Condition.ColorTolerance = (int)numTolerance.Value;
            Condition.MatchPercent = (int)numPercent.Value;

            if (cboTarget.SelectedIndex == TARGET_INDEX_END)
            {
                Condition.Target = ConditionTarget.EndAction;
                Condition.TargetStep = 0;
            }
            else if (cboTarget.SelectedIndex >= TARGET_INDEX_FIRST_STEP)
            {
                Condition.Target = ConditionTarget.GoToStep;
                Condition.TargetStep = cboTarget.SelectedIndex - TARGET_INDEX_FIRST_STEP;
            }
            else
            {
                Condition.Target = ConditionTarget.NextStep;
                Condition.TargetStep = 0;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
