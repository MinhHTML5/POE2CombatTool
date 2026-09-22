using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using POE2Tools.Modules;
using POE2Tools.Utilities;

namespace POE2Tools
{
    // Left side: macro presets, a recorded list of clicks and key presses.
    // Right side: action presets, a series of macro presets linked together by conditions.
    public partial class AutomationForm : Form
    {
        private const string NO_MACRO_TEXT = "(None, only wait for the conditions)";
        // Columns of lstActions that can be edited with a double click
        private const int COLUMN_TIME = 1;
        private const int COLUMN_DELTA = 2;
        private const int COLUMN_ACTION = 3;
        private const int COLUMN_DETAIL = 4;

        // The condition taken by the Copy button. It stays there even if the window is closed
        private static ActionCondition _copiedCondition;
        // Same for the step, which comes with its conditions
        private static ActionStep _copiedStep;

        private AutomationModule _automationModule;
        private MacroPresetStore _store;
        private MacroPreset _activePreset;
        // The type being edited, it goes to the preset when saved. The delay being edited is in txtDelay
        private MacroType _workingType = MacroType.Repeat;
        // The "Item pickup" settings being edited, a copy of those of the preset
        private ItemPickupSettings _workingPickup = ItemPickupSettings.CreateDefault();
        // The action preset as it is saved in the store, and the copy that is being edited
        private ActionPreset _activeAction;
        private ActionPreset _workingAction;
        // Set while the code itself changes a dropdown / list selection
        private bool _updatingSelection = false;
        // The box shown over a time of lstActions while it is being edited
        private TextBox _timeEditor;
        private int _timeEditorRow;
        private int _timeEditorColumn;
        // What is currently selected in the lists to show where the playback is, -1 if nothing
        private int _shownStep = -1;
        private int _shownAction = -1;

        public AutomationForm(AutomationModule automationModule)
        {
            _automationModule = automationModule;
            InitializeComponent();
        }

        private void AutomationForm_Load(object sender, EventArgs e)
        {
            _automationModule.StateChanged += OnModuleStateChanged;
            _automationModule.MacroChanged += OnModuleMacroChanged;
            _automationModule.ActionRecorded += OnModuleActionRecorded;
            _automationModule.ActionPlanProvider = BuildActionPlan;
            _automationModule.SetWindowOpen(true);

            cboMacroType.Items.AddRange(MacroPreset.TypeNames);
            pickupEditor.SettingsChanged += UpdateControls;

            _store = MacroPresetStore.Load();
            txtStopAfter.Text = _store.StopAfterCount.ToString();
            chkStopAfter.Checked = _store.StopAfterEnabled;
            chkSleepWhenDone.Checked = _store.SleepWhenDone;
            ApplyStopAfter();
            RefreshPresetList();
            RefreshActionPresetList();

            MacroPreset preset = _store.Find(_store.SelectedPreset);
            if (preset == null && _store.Presets.Count > 0) preset = _store.Presets[0];
            ActivatePreset(preset);

            ActionPreset action = _store.FindAction(_store.SelectedActionPreset);
            if (action == null && _store.ActionPresets.Count > 0) action = _store.ActionPresets[0];
            ActivateAction(action);

            tmrStatus.Start();
        }

        private void AutomationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _automationModule.StopAll();
            if (!ConfirmUnsavedChanges() || !ConfirmUnsavedActionChanges())
            {
                e.Cancel = true;
            }
        }

        private void AutomationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            tmrStatus.Stop();
            _automationModule.StateChanged -= OnModuleStateChanged;
            _automationModule.MacroChanged -= OnModuleMacroChanged;
            _automationModule.ActionRecorded -= OnModuleActionRecorded;
            _automationModule.ActionPlanProvider = null;
            _automationModule.SetWindowOpen(false);

            // Remember the "Stop after" option for the next time
            if (_store != null && (_store.StopAfterEnabled != chkStopAfter.Checked || _store.StopAfterCount != GetStopAfterCount()
                || _store.SleepWhenDone != chkSleepWhenDone.Checked))
            {
                _store.StopAfterEnabled = chkStopAfter.Checked;
                _store.SleepWhenDone = chkSleepWhenDone.Checked;
                if (GetStopAfterCount() > 0) _store.StopAfterCount = GetStopAfterCount();
                _store.Save();
            }
        }

        // Returns 0 if the text is not a valid number of times
        private int GetStopAfterCount()
        {
            return int.TryParse(txtStopAfter.Text, out int count) && count > 0 ? count : 0;
        }

        // Tell the module when the action must stop by itself. It can be changed while the action is running
        private void ApplyStopAfter()
        {
            int count = GetStopAfterCount();
            txtStopAfter.BackColor = count > 0 ? SystemColors.Window : Color.LightCoral;
            _automationModule.StopAfterFinalSteps = chkStopAfter.Checked ? count : 0;
            _automationModule.SleepWhenDone = chkSleepWhenDone.Checked;
        }

        private void chkSleepWhenDone_CheckedChanged(object sender, EventArgs e)
        {
            ApplyStopAfter();
        }

        private void chkStopAfter_CheckedChanged(object sender, EventArgs e)
        {
            ApplyStopAfter();
        }

        private void txtStopAfter_TextChanged(object sender, EventArgs e)
        {
            ApplyStopAfter();
        }



        // ------------------------------------------------------------------------------------
        // Macro preset
        // ------------------------------------------------------------------------------------

        private bool IsDirty()
        {
            return _activePreset != null && (_activePreset.Type != _workingType || _activePreset.DelayMs != GetDelay()
                || MacroPresetStore.ToJson(_activePreset.Pickup) != MacroPresetStore.ToJson(_workingPickup)
                || !_activePreset.IsSameMacro(_automationModule.Actions, _automationModule.Duration));
        }

        // Returns -1 if the text is not a valid delay
        private int GetDelay()
        {
            return int.TryParse(txtDelay.Text, out int delay) && delay > 0 && delay <= AutomationModule.MAX_ACTION_TIME ? delay : -1;
        }

        // Returns false if the user cancelled, meaning the current preset must stay as it is
        private bool ConfirmUnsavedChanges()
        {
            if (!IsDirty()) return true;

            DialogResult result = MessageBox.Show(this,
                "Macro preset \"" + _activePreset.Name + "\" has unsaved changes.\nDo you want to save them?",
                "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Cancel) return false;
            if (result == DialogResult.Yes) return SaveActivePreset();
            return true;
        }

        private bool SaveActivePreset()
        {
            if (_activePreset == null) return false;
            if (GetDelay() < 0)
            {
                MessageBox.Show(this, "The delay must be a number of milliseconds, from 1 to " + AutomationModule.MAX_ACTION_TIME + ".",
                    "Save macro preset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            List<MacroAction> oldActions = _activePreset.Actions;
            int oldDuration = _activePreset.Duration;
            MacroType oldType = _activePreset.Type;
            int oldDelay = _activePreset.DelayMs;
            ItemPickupSettings oldPickup = _activePreset.Pickup;

            _activePreset.Actions = new List<MacroAction>();
            foreach (MacroAction action in _automationModule.Actions)
            {
                _activePreset.Actions.Add(action.Clone());
            }
            _activePreset.Duration = _automationModule.Duration;
            _activePreset.Type = _workingType;
            _activePreset.DelayMs = GetDelay();
            _activePreset.Pickup = MacroPresetStore.ClonePickup(_workingPickup);

            if (!_store.Save())
            {
                _activePreset.Actions = oldActions;
                _activePreset.Duration = oldDuration;
                _activePreset.Type = oldType;
                _activePreset.DelayMs = oldDelay;
                _activePreset.Pickup = oldPickup;
                return false;
            }
            UpdateControls();
            return true;
        }

        private void RefreshPresetList()
        {
            _updatingSelection = true;
            cboPreset.Items.Clear();
            foreach (MacroPreset preset in _store.Presets)
            {
                cboPreset.Items.Add(preset.Name);
            }
            _updatingSelection = false;
        }

        // Load the macro of the preset into the module. Null means there is no preset at all.
        private void ActivatePreset(MacroPreset preset)
        {
            _activePreset = preset;

            _updatingSelection = true;
            cboPreset.SelectedIndex = preset == null ? -1 : _store.Presets.IndexOf(preset);
            _workingType = preset == null ? MacroType.Repeat : preset.Type;
            cboMacroType.SelectedIndex = preset == null ? -1 : (int)_workingType;
            txtDelay.Text = (preset == null ? new MacroPreset() : preset).DelayMs.ToString();
            _workingPickup = preset == null ? ItemPickupSettings.CreateDefault() : MacroPresetStore.ClonePickup(preset.Pickup);
            pickupEditor.SetSettings(_automationModule, _workingPickup);
            _updatingSelection = false;

            _automationModule.LoadMacro(preset == null ? new List<MacroAction>() : preset.Actions, preset == null ? 0 : preset.Duration);
            ApplyMacroType();

            string selectedName = preset == null ? "" : preset.Name;
            if (_store.SelectedPreset != selectedName)
            {
                _store.SelectedPreset = selectedName;
                _store.Save();
            }

            UpdateControls();
        }

        // Show the editor of the working type. Only a "Repeat macro" can be recorded, and a "Delay" can't be played alone
        private void ApplyMacroType()
        {
            bool isRepeat = _workingType == MacroType.Repeat;
            bool isDelay = _workingType == MacroType.Delay;
            bool isPickup = _workingType == MacroType.ItemPickup;
            lblDelay.Visible = isDelay;
            txtDelay.Visible = isDelay;
            lblDelayHelp.Visible = isDelay;
            pickupEditor.Visible = isPickup;
            lblHotkeys.Visible = isRepeat;
            lstActions.Visible = isRepeat;
            lblDuration.Visible = isRepeat;

            _automationModule.SetPickupMacro(_activePreset != null && isPickup ? _workingPickup : null);
            _automationModule.SetMacroSelected(_activePreset != null && !isDelay);
        }

        private void cboMacroType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingSelection || cboMacroType.SelectedIndex < 0) return;

            _workingType = (MacroType)cboMacroType.SelectedIndex;
            ApplyMacroType();
            UpdateControls();
        }

        private void txtDelay_TextChanged(object sender, EventArgs e)
        {
            txtDelay.BackColor = GetDelay() > 0 ? SystemColors.Window : Color.LightCoral;
            if (_updatingSelection) return;
            UpdateControls();
        }

        private void cboPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingSelection) return;

            MacroPreset preset = cboPreset.SelectedIndex >= 0 ? _store.Presets[cboPreset.SelectedIndex] : null;
            if (preset == _activePreset) return;

            if (!ConfirmUnsavedChanges())
            {
                // Cancelled, go back to the previous preset
                _updatingSelection = true;
                cboPreset.SelectedIndex = _store.Presets.IndexOf(_activePreset);
                _updatingSelection = false;
                return;
            }

            ActivatePreset(preset);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (!ConfirmUnsavedChanges()) return;

            string name = PromptForNewName("New macro preset", n => _store.Find(n) != null);
            if (name == null) return;

            MacroPreset preset = new MacroPreset { Name = name };
            _store.Presets.Add(preset);
            _store.SelectedPreset = name;
            if (!_store.Save())
            {
                _store.Presets.Remove(preset);
                return;
            }

            RefreshPresetList();
            ActivatePreset(preset);
            // The steps can now pick this macro
            RefreshSteps(GetSelectedStepIndex());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveActivePreset();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_activePreset == null) return;

            DialogResult result = MessageBox.Show(this,
                "Delete macro preset \"" + _activePreset.Name + "\"?\nIts recorded macro will be lost, and the action presets using it will not run anymore.",
                "Delete macro preset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result != DialogResult.Yes) return;

            int index = _store.Presets.IndexOf(_activePreset);
            _store.Presets.RemoveAt(index);
            _store.SelectedPreset = "";
            if (!_store.Save())
            {
                _store.Presets.Insert(index, _activePreset);
                return;
            }

            RefreshPresetList();
            MacroPreset next = _store.Presets.Count == 0 ? null : _store.Presets[Math.Min(index, _store.Presets.Count - 1)];
            ActivatePreset(next);
            RefreshSteps(GetSelectedStepIndex());
        }

        // Ask for a name until it is valid. Returns null if the user cancelled
        private string PromptForNewName(string title, Func<string, bool> nameExists)
        {
            string name = "";
            while (true)
            {
                name = PromptForName(title, name);
                if (name == null) return null;

                if (name.Length == 0)
                {
                    MessageBox.Show(this, "The preset name can't be empty.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (nameExists(name))
                {
                    MessageBox.Show(this, "There is already a preset named \"" + name + "\".", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    return name;
                }
            }
        }

        // Returns the trimmed name, or null if the user cancelled
        private string PromptForName(string title, string initialName)
        {
            using (Form dialog = new Form())
            using (Label label = new Label())
            using (TextBox textBox = new TextBox())
            using (Button okButton = new Button())
            using (Button cancelButton = new Button())
            {
                dialog.Text = title;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ClientSize = new Size(300, 100);
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ShowInTaskbar = false;

                label.Text = "Preset name:";
                label.AutoSize = true;
                label.Location = new Point(12, 12);

                textBox.Location = new Point(12, 34);
                textBox.Size = new Size(276, 23);
                textBox.MaxLength = 50;
                textBox.Text = initialName;

                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(132, 66);
                okButton.Size = new Size(75, 25);

                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(213, 66);
                cancelButton.Size = new Size(75, 25);

                dialog.Controls.Add(label);
                dialog.Controls.Add(textBox);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                return dialog.ShowDialog(this) == DialogResult.OK ? textBox.Text.Trim() : null;
            }
        }



        // ------------------------------------------------------------------------------------
        // Action preset
        // ------------------------------------------------------------------------------------

        private bool IsActionDirty()
        {
            return _activeAction != null && MacroPresetStore.ToJson(_activeAction) != MacroPresetStore.ToJson(_workingAction);
        }

        private bool ConfirmUnsavedActionChanges()
        {
            if (!IsActionDirty()) return true;

            DialogResult result = MessageBox.Show(this,
                "Action preset \"" + _activeAction.Name + "\" has unsaved changes.\nDo you want to save them?",
                "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Cancel) return false;
            if (result == DialogResult.Yes) return SaveActiveAction();
            return true;
        }

        private bool SaveActiveAction()
        {
            if (_activeAction == null) return false;

            List<ActionStep> oldSteps = _activeAction.Steps;
            _activeAction.Steps = MacroPresetStore.CloneAction(_workingAction).Steps;
            if (!_store.Save())
            {
                _activeAction.Steps = oldSteps;
                return false;
            }
            UpdateControls();
            return true;
        }

        private void RefreshActionPresetList()
        {
            _updatingSelection = true;
            cboActionPreset.Items.Clear();
            foreach (ActionPreset preset in _store.ActionPresets)
            {
                cboActionPreset.Items.Add(preset.Name);
            }
            _updatingSelection = false;
        }

        private void ActivateAction(ActionPreset preset)
        {
            _automationModule.StopAll();
            _activeAction = preset;
            _workingAction = preset == null ? null : MacroPresetStore.CloneAction(preset);

            _updatingSelection = true;
            cboActionPreset.SelectedIndex = preset == null ? -1 : _store.ActionPresets.IndexOf(preset);
            _updatingSelection = false;

            string selectedName = preset == null ? "" : preset.Name;
            if (_store.SelectedActionPreset != selectedName)
            {
                _store.SelectedActionPreset = selectedName;
                _store.Save();
            }

            RefreshSteps(0);
        }

        private void cboActionPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingSelection) return;

            ActionPreset preset = cboActionPreset.SelectedIndex >= 0 ? _store.ActionPresets[cboActionPreset.SelectedIndex] : null;
            if (preset == _activeAction) return;

            if (!ConfirmUnsavedActionChanges())
            {
                _updatingSelection = true;
                cboActionPreset.SelectedIndex = _store.ActionPresets.IndexOf(_activeAction);
                _updatingSelection = false;
                return;
            }

            ActivateAction(preset);
        }

        private void btnActionNew_Click(object sender, EventArgs e)
        {
            if (!ConfirmUnsavedActionChanges()) return;

            string name = PromptForNewName("New action preset", n => _store.FindAction(n) != null);
            if (name == null) return;

            ActionPreset preset = new ActionPreset { Name = name };
            _store.ActionPresets.Add(preset);
            _store.SelectedActionPreset = name;
            if (!_store.Save())
            {
                _store.ActionPresets.Remove(preset);
                return;
            }

            RefreshActionPresetList();
            ActivateAction(preset);
        }

        private void btnActionSave_Click(object sender, EventArgs e)
        {
            SaveActiveAction();
        }

        private void btnActionDelete_Click(object sender, EventArgs e)
        {
            if (_activeAction == null) return;

            DialogResult result = MessageBox.Show(this,
                "Delete action preset \"" + _activeAction.Name + "\"?\nIts steps, conditions and samples will be lost.",
                "Delete action preset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result != DialogResult.Yes) return;

            int index = _store.ActionPresets.IndexOf(_activeAction);
            _store.ActionPresets.RemoveAt(index);
            _store.SelectedActionPreset = "";
            if (!_store.Save())
            {
                _store.ActionPresets.Insert(index, _activeAction);
                return;
            }

            RefreshActionPresetList();
            ActionPreset next = _store.ActionPresets.Count == 0 ? null : _store.ActionPresets[Math.Min(index, _store.ActionPresets.Count - 1)];
            ActivateAction(next);
        }



        private int GetSelectedStepIndex()
        {
            return lstSteps.SelectedIndices.Count > 0 ? lstSteps.SelectedIndices[0] : -1;
        }

        private ActionStep GetSelectedStep()
        {
            int index = GetSelectedStepIndex();
            return _workingAction != null && index >= 0 && index < _workingAction.Steps.Count ? _workingAction.Steps[index] : null;
        }

        private int GetSelectedConditionIndex()
        {
            return lstConditions.SelectedIndices.Count > 0 ? lstConditions.SelectedIndices[0] : -1;
        }

        private string DescribeStepMacro(ActionStep step)
        {
            if (step.MacroName == "") return "(wait only)";
            return _store.Find(step.MacroName) == null ? step.MacroName + " (missing!)" : step.MacroName;
        }

        private string DescribeTarget(ActionCondition condition)
        {
            switch (condition.Target)
            {
                case ConditionTarget.EndAction: return "End the action";
                case ConditionTarget.GoToStep: return "Step " + (condition.TargetStep + 1);
                default: return "Next step";
            }
        }

        // Rebuild the list of steps, then select the given one
        private void RefreshSteps(int selectIndex)
        {
            _updatingSelection = true;
            lstSteps.BeginUpdate();
            lstSteps.Items.Clear();
            if (_workingAction != null)
            {
                for (int i = 0; i < _workingAction.Steps.Count; i++)
                {
                    ActionStep step = _workingAction.Steps[i];
                    List<string> conditions = new List<string>();
                    foreach (ActionCondition condition in step.Conditions)
                    {
                        string when;
                        switch (condition.Type)
                        {
                            case ConditionType.ScreenMatch: when = "Screen " + condition.MatchPercent + "%"; break;
                            case ConditionType.MacroLooped: when = "Looped " + condition.LoopCount; break;
                            case ConditionType.MacroRanFor: when = "Ran " + condition.RunSeconds.ToString("0.#") + "s"; break;
                            default: when = "Ended"; break;
                        }
                        conditions.Add(when + " -> " + DescribeTarget(condition));
                    }

                    ListViewItem item = new ListViewItem((i + 1).ToString());
                    item.SubItems.Add(DescribeStepMacro(step));
                    item.SubItems.Add(conditions.Count == 0 ? "(none, loops forever)" : string.Join(";  ", conditions));
                    lstSteps.Items.Add(item);
                }

                selectIndex = Math.Max(0, Math.Min(selectIndex, lstSteps.Items.Count - 1));
                if (lstSteps.Items.Count > 0)
                {
                    lstSteps.Items[selectIndex].Selected = true;
                    lstSteps.Items[selectIndex].EnsureVisible();
                }
            }
            lstSteps.EndUpdate();
            _updatingSelection = false;

            RefreshStepEditor(0);
        }

        // Show the macro and the conditions of the selected step
        private void RefreshStepEditor(int selectConditionIndex)
        {
            ActionStep step = GetSelectedStep();

            _updatingSelection = true;
            cboStepMacro.Items.Clear();
            lstConditions.Items.Clear();
            if (step != null)
            {
                cboStepMacro.Items.Add(NO_MACRO_TEXT);
                foreach (MacroPreset preset in _store.Presets)
                {
                    cboStepMacro.Items.Add(preset.Name);
                }
                if (step.MacroName == "")
                {
                    cboStepMacro.SelectedIndex = 0;
                }
                else
                {
                    MacroPreset preset = _store.Find(step.MacroName);
                    // Keep showing a deleted macro, so it's clear what has to be replaced
                    cboStepMacro.SelectedIndex = preset != null ? _store.Presets.IndexOf(preset) + 1 : cboStepMacro.Items.Add(DescribeStepMacro(step));
                }

                foreach (ActionCondition condition in step.Conditions)
                {
                    ListViewItem item = new ListViewItem(condition.Describe());
                    item.SubItems.Add(DescribeTarget(condition));
                    lstConditions.Items.Add(item);
                }
                if (lstConditions.Items.Count > 0)
                {
                    selectConditionIndex = Math.Max(0, Math.Min(selectConditionIndex, lstConditions.Items.Count - 1));
                    lstConditions.Items[selectConditionIndex].Selected = true;
                }
            }
            _updatingSelection = false;

            UpdateControls();
        }

        private void lstSteps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingSelection) return;
            RefreshStepEditor(0);
        }

        private void lstConditions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingSelection) return;
            UpdateControls();
        }

        private void btnStepAdd_Click(object sender, EventArgs e)
        {
            if (_workingAction == null) return;

            // The usual step: play the macro once, then move on
            ActionStep step = new ActionStep { MacroName = _activePreset == null ? "" : _activePreset.Name };
            step.Conditions.Add(new ActionCondition());

            int index = GetSelectedStepIndex() + 1;
            if (index <= 0) index = _workingAction.Steps.Count;
            RetargetSteps(target => target >= index ? target + 1 : target);
            _workingAction.Steps.Insert(index, step);
            RefreshSteps(index);
        }

        private void btnStepRemove_Click(object sender, EventArgs e)
        {
            int index = GetSelectedStepIndex();
            if (GetSelectedStep() == null) return;

            _workingAction.Steps.RemoveAt(index);
            // Conditions that were going to the removed step now end the action
            RetargetSteps(target => target == index ? -1 : (target > index ? target - 1 : target));
            RefreshSteps(index);
        }

        private void btnStepCopy_Click(object sender, EventArgs e)
        {
            ActionStep step = GetSelectedStep();
            if (step == null) return;

            _copiedStep = MacroPresetStore.CloneStep(step);
            UpdateControls();
        }

        // Paste under the selected step
        private void btnStepPaste_Click(object sender, EventArgs e)
        {
            if (_workingAction == null || _copiedStep == null) return;

            ActionStep step = MacroPresetStore.CloneStep(_copiedStep);
            int index = GetSelectedStepIndex() + 1;
            if (index <= 0) index = _workingAction.Steps.Count;
            RetargetSteps(target => target >= index ? target + 1 : target);
            _workingAction.Steps.Insert(index, step);

            // It can come from another action preset, which had more steps
            foreach (ActionCondition condition in step.Conditions)
            {
                if (condition.Target == ConditionTarget.GoToStep && condition.TargetStep >= _workingAction.Steps.Count)
                {
                    condition.Target = ConditionTarget.EndAction;
                    condition.TargetStep = 0;
                }
            }
            RefreshSteps(index);
        }

        private void btnStepUp_Click(object sender, EventArgs e)
        {
            MoveSelectedStep(-1);
        }

        private void btnStepDown_Click(object sender, EventArgs e)
        {
            MoveSelectedStep(1);
        }

        private void MoveSelectedStep(int offset)
        {
            int index = GetSelectedStepIndex();
            int other = index + offset;
            if (GetSelectedStep() == null || other < 0 || other >= _workingAction.Steps.Count) return;

            ActionStep step = _workingAction.Steps[index];
            _workingAction.Steps[index] = _workingAction.Steps[other];
            _workingAction.Steps[other] = step;
            // The "go to step" conditions keep following the steps they were pointing at
            RetargetSteps(target => target == index ? other : (target == other ? index : target));
            RefreshSteps(other);
        }

        // Update the "go to step" conditions after the steps moved. A negative result means the step is gone
        private void RetargetSteps(Func<int, int> getNewIndex)
        {
            foreach (ActionStep step in _workingAction.Steps)
            {
                foreach (ActionCondition condition in step.Conditions)
                {
                    if (condition.Target != ConditionTarget.GoToStep) continue;

                    int newIndex = getNewIndex(condition.TargetStep);
                    if (newIndex < 0)
                    {
                        condition.Target = ConditionTarget.EndAction;
                        newIndex = 0;
                    }
                    condition.TargetStep = newIndex;
                }
            }
        }

        private void cboStepMacro_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActionStep step = GetSelectedStep();
            if (_updatingSelection || step == null) return;

            int index = cboStepMacro.SelectedIndex;
            // The last item can be the deleted macro the step is still using, that's not a change
            if (index > _store.Presets.Count) return;

            step.MacroName = index <= 0 ? "" : _store.Presets[index - 1].Name;
            RefreshSteps(GetSelectedStepIndex());
        }

        // Returns the edited condition, or null if the user cancelled
        private ActionCondition EditCondition(ActionCondition condition)
        {
            List<string> stepNames = new List<string>();
            foreach (ActionStep step in _workingAction.Steps)
            {
                stepNames.Add(DescribeStepMacro(step));
            }

            using (ConditionForm conditionForm = new ConditionForm(_automationModule, condition, stepNames))
            {
                return conditionForm.ShowDialog(this) == DialogResult.OK ? conditionForm.Condition : null;
            }
        }

        private void btnConditionAdd_Click(object sender, EventArgs e)
        {
            ActionStep step = GetSelectedStep();
            if (step == null) return;

            ActionCondition condition = EditCondition(new ActionCondition { Type = ConditionType.ScreenMatch });
            if (condition == null) return;

            step.Conditions.Add(condition);
            int conditionIndex = step.Conditions.Count - 1;
            RefreshSteps(GetSelectedStepIndex());
            RefreshStepEditor(conditionIndex);
        }

        private void btnConditionEdit_Click(object sender, EventArgs e)
        {
            ActionStep step = GetSelectedStep();
            int conditionIndex = GetSelectedConditionIndex();
            if (step == null || conditionIndex < 0) return;

            // Edit a copy, so cancelling leaves the condition untouched
            ActionCondition condition = EditCondition(step.Conditions[conditionIndex].Clone());
            if (condition == null) return;

            step.Conditions[conditionIndex] = condition;
            RefreshSteps(GetSelectedStepIndex());
            RefreshStepEditor(conditionIndex);
        }

        private void lstConditions_DoubleClick(object sender, EventArgs e)
        {
            if (btnConditionEdit.Enabled) btnConditionEdit_Click(sender, e);
        }

        private void btnConditionRemove_Click(object sender, EventArgs e)
        {
            ActionStep step = GetSelectedStep();
            int conditionIndex = GetSelectedConditionIndex();
            if (step == null || conditionIndex < 0) return;

            step.Conditions.RemoveAt(conditionIndex);
            RefreshSteps(GetSelectedStepIndex());
            RefreshStepEditor(conditionIndex);
        }

        private void btnConditionCopy_Click(object sender, EventArgs e)
        {
            ActionStep step = GetSelectedStep();
            int conditionIndex = GetSelectedConditionIndex();
            if (step == null || conditionIndex < 0) return;

            _copiedCondition = step.Conditions[conditionIndex].Clone();
            UpdateControls();
        }

        // Paste under the selected condition
        private void btnConditionPaste_Click(object sender, EventArgs e)
        {
            ActionStep step = GetSelectedStep();
            if (step == null || _copiedCondition == null) return;

            ActionCondition condition = _copiedCondition.Clone();
            // It can come from another action preset, which had more steps
            if (condition.Target == ConditionTarget.GoToStep && condition.TargetStep >= _workingAction.Steps.Count)
            {
                condition.Target = ConditionTarget.EndAction;
                condition.TargetStep = 0;
            }

            int conditionIndex = GetSelectedConditionIndex() + 1;
            if (conditionIndex <= 0) conditionIndex = step.Conditions.Count;
            step.Conditions.Insert(conditionIndex, condition);
            RefreshSteps(GetSelectedStepIndex());
            RefreshStepEditor(conditionIndex);
        }

        // Called by the module when Ctrl + Down is pressed: turn what is on screen into something it can run.
        // Returns null and an error message if the action preset can't run.
        // Returns null and no error if there is no action at all, the macro preset then plays alone.
        private List<AutomationStep> BuildActionPlan(out string error)
        {
            error = "";
            if (_workingAction == null || _workingAction.Steps.Count == 0)
            {
                return null;
            }

            List<AutomationStep> plan = new List<AutomationStep>();
            for (int i = 0; i < _workingAction.Steps.Count && error == ""; i++)
            {
                ActionStep step = _workingAction.Steps[i];
                AutomationStep planStep = new AutomationStep { Name = step.MacroName };
                plan.Add(planStep);

                if (step.MacroName != "")
                {
                    MacroPreset preset = _store.Find(step.MacroName);
                    if (preset == null)
                    {
                        error = "Step " + (i + 1) + ": macro preset \"" + step.MacroName + "\" doesn't exist anymore.";
                        break;
                    }

                    // The macro being edited on the left plays as it is shown, even if not saved yet
                    bool isWorkingMacro = preset == _activePreset;
                    MacroType type = isWorkingMacro ? _workingType : preset.Type;
                    if (type == MacroType.ItemPickup)
                    {
                        planStep.Pickup = MacroPresetStore.ClonePickup(isWorkingMacro ? _workingPickup : preset.Pickup);
                        if (planStep.Pickup.Labels.Count == 0)
                        {
                            error = "Step " + (i + 1) + ": macro preset \"" + preset.Name + "\" has no label color to look for.";
                            break;
                        }
                    }
                    else if (type == MacroType.Delay)
                    {
                        // A macro without any action, that ends after the delay
                        planStep.Duration = isWorkingMacro ? GetDelay() : preset.DelayMs;
                        if (planStep.Duration < 0)
                        {
                            error = "Step " + (i + 1) + ": the delay of macro preset \"" + preset.Name + "\" is not valid.";
                            break;
                        }
                    }
                    else
                    {
                        foreach (MacroAction action in isWorkingMacro ? _automationModule.Actions : preset.Actions)
                        {
                            planStep.Actions.Add(action.Clone());
                        }
                        planStep.Duration = isWorkingMacro ? _automationModule.Duration : preset.Duration;
                    }
                }

                foreach (ActionCondition condition in step.Conditions)
                {
                    int target = AutomationStep.TARGET_END;
                    if (condition.Target == ConditionTarget.NextStep && i + 1 < _workingAction.Steps.Count) target = i + 1;
                    if (condition.Target == ConditionTarget.GoToStep)
                    {
                        if (condition.TargetStep < 0 || condition.TargetStep >= _workingAction.Steps.Count)
                        {
                            error = "Step " + (i + 1) + ": a condition goes to step " + (condition.TargetStep + 1) + ", which doesn't exist.";
                            break;
                        }
                        target = condition.TargetStep;
                    }

                    if (condition.Type != ConditionType.ScreenMatch)
                    {
                        // "Ended" is the same as "looped 1 time"
                        AutomationEndCheck endCheck = new AutomationEndCheck { Loops = 1, Target = target };
                        if (condition.Type == ConditionType.MacroLooped) endCheck.Loops = Math.Max(1, condition.LoopCount);
                        if (condition.Type == ConditionType.MacroRanFor) endCheck.Milliseconds = (long)(condition.RunSeconds * 1000);
                        planStep.EndChecks.Add(endCheck);
                        continue;
                    }

                    using (Bitmap sample = ScreenMatcher.FromBase64(condition.SampleBase64))
                    {
                        if (sample == null)
                        {
                            error = "Step " + (i + 1) + ": a screen condition has no sample.";
                            break;
                        }
                        planStep.Checks.Add(new AutomationScreenCheck
                        {
                            Matcher = new ScreenMatcher(sample, condition.Region),
                            Interval = Math.Max(1, condition.CheckInterval),
                            Tolerance = condition.ColorTolerance,
                            MatchPercent = condition.MatchPercent,
                            Target = target
                        });
                    }
                }
            }

            if (error != "")
            {
                foreach (AutomationStep planStep in plan)
                {
                    foreach (AutomationScreenCheck check in planStep.Checks)
                    {
                        check.Matcher.Dispose();
                    }
                }
                return null;
            }
            return plan;
        }



        // ------------------------------------------------------------------------------------
        // Module events and status
        // ------------------------------------------------------------------------------------

        private void OnModuleStateChanged()
        {
            UpdateControls();
        }

        private void OnModuleMacroChanged()
        {
            EndTimeEdit(false);
            lstActions.BeginUpdate();
            lstActions.Items.Clear();
            foreach (MacroAction action in _automationModule.Actions)
            {
                lstActions.Items.Add(CreateListItem(action));
            }
            lstActions.EndUpdate();
            UpdateControls();
        }

        private void OnModuleActionRecorded(MacroAction action)
        {
            ListViewItem item = lstActions.Items.Add(CreateListItem(action));
            item.EnsureVisible();
        }

        private void lstActions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_automationModule.IsRecording || _automationModule.IsPlaying) return;

            ListViewHitTestInfo hit = lstActions.HitTest(e.Location);
            if (hit.Item == null || hit.SubItem == null) return;

            int column = hit.Item.SubItems.IndexOf(hit.SubItem);
            int row = hit.Item.Index;
            if (column == COLUMN_TIME || column == COLUMN_DELTA)
            {
                BeginTimeEdit(row, column, hit.SubItem.Bounds);
            }
            else if ((column == COLUMN_ACTION || column == COLUMN_DETAIL) && row < _automationModule.Actions.Count)
            {
                MacroAction action = _automationModule.Actions[row];
                bool changed;
                if (action.IsMouse)
                {
                    changed = PromptForPosition(action, out double xPercent, out double yPercent)
                        && _automationModule.SetActionPosition(row, xPercent, yPercent);
                }
                else
                {
                    Keys key = PromptForKey(action);
                    changed = key != Keys.None && _automationModule.SetActionKey(row, key);
                }
                if (changed) SelectActionRow(row);
            }
        }

        // The list was rebuilt, go back to the edited row
        private void SelectActionRow(int row)
        {
            if (lstActions.Items.Count == 0) return;

            row = Math.Max(0, Math.Min(row, lstActions.Items.Count - 1));
            lstActions.Items[row].Selected = true;
            lstActions.Items[row].EnsureVisible();
        }

        private void lstActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            // The playback also moves the selection, many times per second
            if (_automationModule.IsPlaying) return;
            UpdateControls();
        }

        private void btnMacroStepDelete_Click(object sender, EventArgs e)
        {
            if (lstActions.SelectedIndices.Count == 0) return;

            int row = lstActions.SelectedIndices[0];
            if (_automationModule.RemoveAction(row)) SelectActionRow(row);
        }

        // A form that takes every key for itself, even the ones a dialog normally uses (Enter, Escape, Tab, Alt...)
        private class KeyCaptureDialog : Form
        {
            public Keys CapturedKey = Keys.None;

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                CapturedKey = keyData & Keys.KeyCode;
                DialogResult = DialogResult.OK;
                return true;
            }
        }

        private static void SetupPromptDialog(Form dialog, Label label, Button cancelButton, string title, string text)
        {
            dialog.Text = title;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ClientSize = new Size(320, 105);
            dialog.MaximizeBox = false;
            dialog.MinimizeBox = false;
            dialog.ShowInTaskbar = false;

            label.Text = text;
            label.Location = new Point(12, 12);
            label.Size = new Size(296, 50);

            cancelButton.Text = "Cancel";
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(233, 70);
            cancelButton.Size = new Size(75, 25);
            // Space / Enter must not press it, they are keys to capture
            cancelButton.TabStop = false;

            dialog.Controls.Add(label);
            dialog.Controls.Add(cancelButton);
        }

        // Returns Keys.None if the user cancelled
        private Keys PromptForKey(MacroAction action)
        {
            using (KeyCaptureDialog dialog = new KeyCaptureDialog())
            using (Label label = new Label())
            using (Button cancelButton = new Button())
            {
                SetupPromptDialog(dialog, label, cancelButton, "Change the key",
                    "Press the key that replaces \"" + action.Key + "\".\nThe other half of the press (down / up) changes with it.");
                return dialog.ShowDialog(this) == DialogResult.OK ? dialog.CapturedKey : Keys.None;
            }
        }

        // Wait for a click anywhere on the screen, the game included. Returns false if the user cancelled
        private bool PromptForPosition(MacroAction action, out double xPercent, out double yPercent)
        {
            double newX = 0, newY = 0;
            using (Form dialog = new Form())
            using (Label label = new Label())
            using (Button cancelButton = new Button())
            {
                SetupPromptDialog(dialog, label, cancelButton, "Change the position",
                    "Click where the mouse " + (action.Type == MacroActionType.MouseDown ? "down" : "up") + " must happen, anywhere on the screen.\n"
                    + "Now: " + action.DescribeDetail());
                // Stays visible when the click gives the focus to the game
                dialog.TopMost = true;

                Action<MouseButtons, bool> onMouseInput = (button, isDown) =>
                {
                    // The clicks on this dialog are for its Cancel button or to move it
                    if (!isDown || dialog.DialogResult != DialogResult.None || dialog.Bounds.Contains(Cursor.Position)) return;

                    _automationModule._inputHook.GetCurrentMousePercentPosition(out newX, out newY);
                    dialog.DialogResult = DialogResult.OK;
                };

                _automationModule.MouseInput += onMouseInput;
                try
                {
                    bool ok = dialog.ShowDialog(this) == DialogResult.OK;
                    xPercent = newX;
                    yPercent = newY;
                    // The click probably went to another window
                    if (ok) Activate();
                    return ok;
                }
                finally
                {
                    _automationModule.MouseInput -= onMouseInput;
                }
            }
        }

        // Show a text box right over the time, with the number of milliseconds in it
        private void BeginTimeEdit(int row, int column, Rectangle bounds)
        {
            EndTimeEdit(false);

            MacroAction action = _automationModule.Actions[row];
            int previousTime = row > 0 ? _automationModule.Actions[row - 1].Time : 0;

            _timeEditorRow = row;
            _timeEditorColumn = column;
            _timeEditor = new TextBox();
            _timeEditor.MaxLength = 8;
            _timeEditor.Text = (column == COLUMN_TIME ? action.Time : action.Time - previousTime).ToString();
            _timeEditor.Bounds = bounds;
            _timeEditor.KeyDown += timeEditor_KeyDown;
            _timeEditor.Leave += timeEditor_Leave;
            lstActions.Controls.Add(_timeEditor);
            _timeEditor.SelectAll();
            _timeEditor.Focus();
        }

        private void EndTimeEdit(bool commit)
        {
            if (_timeEditor == null) return;

            TextBox editor = _timeEditor;
            // Removing the box makes it lose the focus, which must not end the edit a second time
            _timeEditor = null;
            string text = editor.Text.Trim();
            lstActions.Controls.Remove(editor);
            editor.Dispose();

            if (!commit || _timeEditorRow >= _automationModule.Actions.Count) return;
            if (!int.TryParse(text, out int value) || value < 0)
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            // Both columns move the action and everything after it, they only count from a different place
            int previousTime = _timeEditorRow > 0 ? _automationModule.Actions[_timeEditorRow - 1].Time : 0;
            long newTime = _timeEditorColumn == COLUMN_TIME ? value : (long)previousTime + value;
            int row = _timeEditorRow;
            if (_automationModule.SetActionTime(row, (int)Math.Min(newTime, int.MaxValue)) && row < lstActions.Items.Count)
            {
                // The list was rebuilt, go back to the edited row
                lstActions.Items[row].Selected = true;
                lstActions.Items[row].EnsureVisible();
            }
        }

        private void timeEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                // No "ding"
                e.Handled = true;
                e.SuppressKeyPress = true;
                EndTimeEdit(e.KeyCode == Keys.Enter);
                lstActions.Focus();
            }
        }

        private void timeEditor_Leave(object sender, EventArgs e)
        {
            EndTimeEdit(true);
        }

        private ListViewItem CreateListItem(MacroAction action)
        {
            int index = lstActions.Items.Count;
            int previousTime = index > 0 && index <= _automationModule.Actions.Count ? _automationModule.Actions[index - 1].Time : 0;

            ListViewItem item = new ListViewItem((index + 1).ToString());
            item.SubItems.Add(action.Time + " ms");
            item.SubItems.Add((action.Time - previousTime) + " ms");
            item.SubItems.Add(action.Describe());
            item.SubItems.Add(action.DescribeDetail());
            return item;
        }

        private void UpdateControls()
        {
            // Nothing can be changed in the middle of a recording or a playback
            bool idle = !_automationModule.IsRecording && !_automationModule.IsPlaying;
            cboPreset.Enabled = idle;
            btnNew.Enabled = idle;
            btnDelete.Enabled = idle && _activePreset != null;
            btnSave.Enabled = idle && IsDirty();
            cboMacroType.Enabled = idle && _activePreset != null;
            txtDelay.Enabled = idle && _activePreset != null;
            pickupEditor.Enabled = idle && _activePreset != null;

            bool canEditAction = idle && _workingAction != null;
            bool stepSelected = canEditAction && GetSelectedStep() != null;
            bool conditionSelected = stepSelected && GetSelectedConditionIndex() >= 0;
            cboActionPreset.Enabled = idle;
            btnActionNew.Enabled = idle;
            btnActionDelete.Enabled = idle && _activeAction != null;
            btnActionSave.Enabled = idle && IsActionDirty();
            // Stays enabled while running, a disabled list doesn't show which step is selected
            lstSteps.Enabled = _workingAction != null;
            btnStepAdd.Enabled = canEditAction;
            btnStepRemove.Enabled = stepSelected;
            btnStepUp.Enabled = stepSelected && GetSelectedStepIndex() > 0;
            btnStepDown.Enabled = stepSelected && GetSelectedStepIndex() < _workingAction.Steps.Count - 1;
            btnStepCopy.Enabled = stepSelected;
            btnStepPaste.Enabled = canEditAction && _copiedStep != null;
            cboStepMacro.Enabled = stepSelected;
            lstConditions.Enabled = stepSelected;
            btnConditionAdd.Enabled = stepSelected;
            btnConditionEdit.Enabled = conditionSelected;
            btnConditionRemove.Enabled = conditionSelected;
            btnConditionCopy.Enabled = conditionSelected;
            btnConditionPaste.Enabled = stepSelected && _copiedCondition != null;
            btnMacroStepDelete.Visible = _workingType == MacroType.Repeat;
            btnMacroStepDelete.Enabled = idle && lstActions.SelectedIndices.Count > 0;

            lblDuration.Text = "Loop length: " + _automationModule.Duration + " ms";
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            string text;
            Color color = SystemColors.ControlText;
            if (_automationModule.IsRecording)
            {
                text = "RECORDING... " + _automationModule.Actions.Count + " actions. Press Ctrl + Up to stop.";
                color = Color.Red;
            }
            else if (_automationModule.IsRunningAction)
            {
                int stepIndex = _automationModule.CurrentStep;
                string stepName = _workingAction != null && stepIndex < _workingAction.Steps.Count ? DescribeStepMacro(_workingAction.Steps[stepIndex]) : "";
                string finalSteps = "final step done " + _automationModule.FinalStepCount
                    + (_automationModule.StopAfterFinalSteps > 0 ? " / " + _automationModule.StopAfterFinalSteps : "") + " times";
                text = "RUNNING ACTION, step " + (stepIndex + 1) + " \"" + stepName + "\", loop " + _automationModule.LoopCount + ", " + finalSteps
                    + ". Press Ctrl + Down to stop.\n" + _automationModule.LastCheckInfo;
                color = Color.Green;
            }
            else if (_automationModule.IsPlaying)
            {
                text = "PLAYING MACRO, loop " + _automationModule.LoopCount + ". Press Ctrl + Down to stop.\n" + _automationModule.LastCheckInfo;
                color = Color.Green;
            }
            else if (!_automationModule.IsToolboxStarted)
            {
                text = "The toolbox is OFF. Start it (Ctrl + B) to record or play.";
            }
            else if (_activePreset == null && _activeAction == null)
            {
                text = "Create a preset with the New buttons to use the automation.";
            }
            else
            {
                text = "Ready.";
            }

            if (!_automationModule.IsRecording && !_automationModule.IsPlaying && _automationModule.LastMessage != "")
            {
                text = _automationModule.LastMessage + "\n" + text;
            }

            lblStatus.ForeColor = color;
            lblStatus.Text = text;
        }

        private void tmrStatus_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
            UpdateRunningSelection();
        }

        // While playing, select the running step on the right, and the macro action being played on the left
        private void UpdateRunningSelection()
        {
            if (!_automationModule.IsPlaying)
            {
                _shownStep = -1;
                _shownAction = -1;
                return;
            }

            // Playing the macro preset alone: what is playing is always the macro on the left
            bool showAction = !_automationModule.IsRunningAction;

            if (_automationModule.IsRunningAction && _workingAction != null)
            {
                int stepIndex = _automationModule.CurrentStep;
                if (stepIndex != _shownStep && stepIndex >= 0 && stepIndex < lstSteps.Items.Count)
                {
                    _shownStep = stepIndex;
                    _shownAction = -1;

                    _updatingSelection = true;
                    lstSteps.Items[stepIndex].Selected = true;
                    lstSteps.Items[stepIndex].EnsureVisible();
                    _updatingSelection = false;
                    RefreshStepEditor(0);
                }

                // The running step plays the macro preset shown on the left, its actions can be followed too
                showAction = _activePreset != null && stepIndex >= 0 && stepIndex < _workingAction.Steps.Count
                    && string.Equals(_workingAction.Steps[stepIndex].MacroName, _activePreset.Name, StringComparison.OrdinalIgnoreCase);
            }

            int actionIndex = _automationModule.CurrentActionIndex;
            if (showAction && actionIndex != _shownAction && actionIndex >= 0 && actionIndex < lstActions.Items.Count)
            {
                _shownAction = actionIndex;
                lstActions.Items[actionIndex].Selected = true;
                lstActions.Items[actionIndex].EnsureVisible();
            }
        }
    }
}
