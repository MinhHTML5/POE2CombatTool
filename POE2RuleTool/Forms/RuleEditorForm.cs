using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using POE2RuleTool.Models;

namespace POE2RuleTool.Forms;

public sealed class RuleEditorForm : Form
{
    private readonly RuleDefinition _rule;
    private readonly BindingList<ConditionRow> _conditionRows = new();
    private readonly BindingList<ActionRow> _actionRows = new();

    private readonly TextBox _txtName = new() { Width = 260 };
    private readonly CheckBox _chkEnabled = new() { Text = "Enabled", Width = 82 };
    private readonly ComboBox _cmbMode = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 90 };
    private readonly NumericUpDown _numCooldown = new() { Minimum = 0, Maximum = 600000, Increment = 100, Width = 90 };
    private readonly NumericUpDown _numDefaultTolerance = new() { Minimum = 0, Maximum = 255, Value = 5, Width = 64 };

    private readonly DataGridView _conditionGrid = new();
    private readonly DataGridView _actionGrid = new();
    private readonly ComboBox _cmbActionKind = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
    private readonly TextBox _txtKey = new() { ReadOnly = true, Width = 95, TabStop = true };
    private readonly ComboBox _cmbMouseButton = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 112 };
    private readonly CheckBox _chkCtrl = new() { Text = "Ctrl", Width = 52 };
    private readonly NumericUpDown _numActionDelay = new() { Minimum = 0, Maximum = 600000, Increment = 50, Width = 86 };

    private Keys _selectedKey = Keys.None;

    public RuleEditorForm(RuleDefinition? rule)
    {
        _rule = rule?.Clone() ?? new RuleDefinition();
        EditedRule = _rule.Clone();

        InitializeComponent();
        LoadRuleIntoControls();
    }

    public RuleDefinition EditedRule { get; private set; }

    private void InitializeComponent()
    {
        Text = "Rule Editor";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(980, 660);
        MinimumSize = new Size(880, 560);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(10)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildEditorSplit(), 0, 1);
        root.Controls.Add(BuildFooter(), 0, 2);
        Controls.Add(root);

        AcceptButton = null;
        CancelButton = null;
    }

    private Control BuildHeader()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0, 8, 0, 0)
        };

        _cmbMode.DataSource = Enum.GetValues<ConditionJoinMode>();

        panel.Controls.Add(FormLabel("Name"));
        panel.Controls.Add(_txtName);
        panel.Controls.Add(_chkEnabled);
        panel.Controls.Add(FormLabel("Mode"));
        panel.Controls.Add(_cmbMode);
        panel.Controls.Add(FormLabel("Cooldown"));
        panel.Controls.Add(_numCooldown);
        panel.Controls.Add(FormLabel("ms"));

        return panel;
    }

    private Control BuildEditorSplit()
    {
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 60
        };

        split.Panel1.Controls.Add(BuildConditionsPanel());
        split.Panel2.Controls.Add(BuildActionsPanel());
        return split;
    }

    private Control BuildConditionsPanel()
    {
        var group = new GroupBox { Text = "Conditions", Dock = DockStyle.Fill };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(8) };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        _conditionGrid.Dock = DockStyle.Fill;
        _conditionGrid.AutoGenerateColumns = false;
        _conditionGrid.AllowUserToAddRows = false;
        _conditionGrid.AllowUserToDeleteRows = false;
        _conditionGrid.MultiSelect = true;
        _conditionGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _conditionGrid.RowHeadersVisible = false;
        _conditionGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _conditionGrid.DataSource = _conditionRows;
        _conditionGrid.DataError += ConditionGrid_DataError;

        _conditionGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "X", DataPropertyName = nameof(ConditionRow.X), ReadOnly = true, Width = 72, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
        _conditionGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Y", DataPropertyName = nameof(ConditionRow.Y), ReadOnly = true, Width = 72, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
        _conditionGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tolerance", DataPropertyName = nameof(ConditionRow.Tolerance), Width = 86, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
        _conditionGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sample", DataPropertyName = nameof(ConditionRow.Sample), ReadOnly = true });

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        buttons.Controls.Add(FormLabel("Tolerance"));
        buttons.Controls.Add(_numDefaultTolerance);
        buttons.Controls.Add(CreateButton("Pick Points", PickPoints_Click));
        buttons.Controls.Add(CreateButton("Remove", RemoveSelectedConditions_Click));

        layout.Controls.Add(_conditionGrid, 0, 0);
        layout.Controls.Add(buttons, 0, 1);
        group.Controls.Add(layout);
        return group;
    }

    private Control BuildActionsPanel()
    {
        var group = new GroupBox { Text = "Actions", Dock = DockStyle.Fill };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(8) };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));

        _actionGrid.Dock = DockStyle.Fill;
        _actionGrid.AutoGenerateColumns = false;
        _actionGrid.AllowUserToAddRows = false;
        _actionGrid.AllowUserToDeleteRows = false;
        _actionGrid.ReadOnly = true;
        _actionGrid.MultiSelect = false;
        _actionGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _actionGrid.RowHeadersVisible = false;
        _actionGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _actionGrid.DataSource = _actionRows;
        _actionGrid.SelectionChanged += ActionGrid_SelectionChanged;

        _actionGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Type", DataPropertyName = nameof(ActionRow.Kind), Width = 135, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
        _actionGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Input", DataPropertyName = nameof(ActionRow.Input) });
        _actionGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Delay", DataPropertyName = nameof(ActionRow.Delay), Width = 82, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });

        _cmbActionKind.DataSource = Enum.GetValues<RuleActionKind>();
        _cmbActionKind.SelectedIndexChanged += CmbActionKind_SelectedIndexChanged;
        _cmbMouseButton.DataSource = new[]
        {
            MouseButtons.Left,
            MouseButtons.Right,
            MouseButtons.Middle,
            MouseButtons.XButton1,
            MouseButtons.XButton2
        };
        _txtKey.KeyDown += CaptureActionKey;

        var editor = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
        editor.Controls.Add(FormLabel("Type"));
        editor.Controls.Add(_cmbActionKind);
        editor.Controls.Add(FormLabel("Key"));
        editor.Controls.Add(_txtKey);
        editor.Controls.Add(FormLabel("Mouse"));
        editor.Controls.Add(_cmbMouseButton);
        editor.Controls.Add(_chkCtrl);
        editor.Controls.Add(FormLabel("Delay"));
        editor.Controls.Add(_numActionDelay);
        editor.Controls.Add(FormLabel("ms"));
        editor.Controls.Add(CreateButton("Add", AddAction_Click));
        editor.Controls.Add(CreateButton("Update", UpdateSelectedAction_Click));
        editor.Controls.Add(CreateButton("Remove", RemoveSelectedAction_Click));
        editor.Controls.Add(CreateButton("Up", MoveSelectedActionUp_Click));
        editor.Controls.Add(CreateButton("Down", MoveSelectedActionDown_Click));

        layout.Controls.Add(_actionGrid, 0, 0);
        layout.Controls.Add(editor, 0, 1);
        group.Controls.Add(layout);
        return group;
    }

    private Control BuildFooter()
    {
        var footer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 8, 0, 0)
        };

        var ok = new Button { Text = "OK", Width = 90, Height = 30 };
        ok.Click += Ok_Click;

        var cancel = new Button { Text = "Cancel", Width = 90, Height = 30 };
        cancel.Click += Cancel_Click;


        footer.Controls.Add(ok);
        footer.Controls.Add(cancel);
        return footer;
    }

    private static Label FormLabel(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(8, 6, 4, 0)
        };
    }

    private static Button CreateButton(string text, EventHandler clickHandler)
    {
        var button = new Button { Text = text, Width = 92, Height = 28, Margin = new Padding(8, 0, 0, 0) };
        button.Click += clickHandler;
        return button;
    }
    private void ConditionGrid_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.ThrowException = false;
    }

    private void ActionGrid_SelectionChanged(object? sender, EventArgs e)
    {
        PopulateActionInputsFromSelection();
    }

    private void CmbActionKind_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateActionInputVisibility();
    }

    private void Ok_Click(object? sender, EventArgs e)
    {
        SaveAndClose();
    }

    private void Cancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void PickPoints_Click(object? sender, EventArgs e)
    {
        PickPoints();
    }

    private void RemoveSelectedConditions_Click(object? sender, EventArgs e)
    {
        RemoveSelectedConditions();
    }

    private void AddAction_Click(object? sender, EventArgs e)
    {
        AddAction();
    }

    private void UpdateSelectedAction_Click(object? sender, EventArgs e)
    {
        UpdateSelectedAction();
    }

    private void RemoveSelectedAction_Click(object? sender, EventArgs e)
    {
        RemoveSelectedAction();
    }

    private void MoveSelectedActionUp_Click(object? sender, EventArgs e)
    {
        MoveSelectedActionUp();
    }

    private void MoveSelectedActionDown_Click(object? sender, EventArgs e)
    {
        MoveSelectedActionDown();
    }

    private void LoadRuleIntoControls()
    {
        _txtName.Text = _rule.Name;
        _chkEnabled.Checked = _rule.Enabled;
        _cmbMode.SelectedItem = _rule.ConditionMode;
        _numCooldown.Value = Math.Clamp(_rule.CooldownMs, (int)_numCooldown.Minimum, (int)_numCooldown.Maximum);

        RefreshConditionRows();
        RefreshActionRows();
        UpdateActionInputVisibility();
    }

    private void SaveAndClose()
    {
        string ruleName = _txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(ruleName))
        {
            MessageBox.Show(this, "Rule name is required.", "Rule Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_rule.Conditions.Count == 0)
        {
            MessageBox.Show(this, "Add at least one condition point.", "Rule Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_rule.Actions.Count == 0)
        {
            MessageBox.Show(this, "Add at least one action.", "Rule Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _rule.Name = ruleName;
        _rule.Enabled = _chkEnabled.Checked;
        _rule.ConditionMode = (ConditionJoinMode)_cmbMode.SelectedItem!;
        _rule.CooldownMs = (int)_numCooldown.Value;

        EditedRule = _rule.Clone();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void PickPoints()
    {
        using var picker = new PointPickerForm();
        if (picker.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        foreach (Point point in picker.PickedPoints)
        {
            var condition = new PixelCondition
            {
                X = point.X,
                Y = point.Y,
                Tolerance = (int)_numDefaultTolerance.Value
            };
            _rule.Conditions.Add(condition);
        }

        RefreshConditionRows();
    }

    private void RemoveSelectedConditions()
    {
        foreach (DataGridViewRow gridRow in _conditionGrid.SelectedRows.Cast<DataGridViewRow>().OrderByDescending(row => row.Index))
        {
            if (gridRow.DataBoundItem is ConditionRow row)
            {
                _rule.Conditions.Remove(row.Condition);
            }
        }

        RefreshConditionRows();
    }

    private void AddAction()
    {
        RuleActionDefinition? action = ReadActionInputs();
        if (action == null)
        {
            return;
        }

        _rule.Actions.Add(action);
        RefreshActionRows();
        SelectActionIndex(_rule.Actions.Count - 1);
    }

    private void UpdateSelectedAction()
    {
        int index = SelectedActionIndex();
        if (index < 0)
        {
            return;
        }

        RuleActionDefinition? action = ReadActionInputs();
        if (action == null)
        {
            return;
        }

        _rule.Actions[index] = action;
        RefreshActionRows();
        SelectActionIndex(index);
    }

    private void RemoveSelectedAction()
    {
        int index = SelectedActionIndex();
        if (index < 0)
        {
            return;
        }

        _rule.Actions.RemoveAt(index);
        RefreshActionRows();
        SelectActionIndex(Math.Min(index, _rule.Actions.Count - 1));
    }

    private void MoveSelectedActionUp()
    {
        int index = SelectedActionIndex();
        if (index <= 0)
        {
            return;
        }

        (_rule.Actions[index - 1], _rule.Actions[index]) = (_rule.Actions[index], _rule.Actions[index - 1]);
        RefreshActionRows();
        SelectActionIndex(index - 1);
    }

    private void MoveSelectedActionDown()
    {
        int index = SelectedActionIndex();
        if (index < 0 || index >= _rule.Actions.Count - 1)
        {
            return;
        }

        (_rule.Actions[index + 1], _rule.Actions[index]) = (_rule.Actions[index], _rule.Actions[index + 1]);
        RefreshActionRows();
        SelectActionIndex(index + 1);
    }

    private RuleActionDefinition? ReadActionInputs()
    {
        RuleActionKind kind = (RuleActionKind)_cmbActionKind.SelectedItem!;
        if (kind == RuleActionKind.KeyboardKeyPress && _selectedKey == Keys.None)
        {
            MessageBox.Show(this, "Select a keyboard key for the action.", "Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        return new RuleActionDefinition
        {
            Kind = kind,
            Key = kind == RuleActionKind.KeyboardKeyPress ? _selectedKey : Keys.None,
            MouseButton = kind == RuleActionKind.MouseButtonPress ? (MouseButtons)_cmbMouseButton.SelectedItem! : MouseButtons.Left,
            ControlModifier = _chkCtrl.Checked,
            DelayAfterMs = (int)_numActionDelay.Value
        };
    }

    private void CaptureActionKey(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.ControlKey or Keys.ShiftKey or Keys.Menu)
        {
            return;
        }

        _selectedKey = e.KeyCode;
        _txtKey.Text = e.KeyCode.ToString();
        if (e.Control)
        {
            _chkCtrl.Checked = true;
        }

        e.SuppressKeyPress = true;
    }

    private void PopulateActionInputsFromSelection()
    {
        int index = SelectedActionIndex();
        if (index < 0 || index >= _rule.Actions.Count)
        {
            return;
        }

        RuleActionDefinition action = _rule.Actions[index];
        _cmbActionKind.SelectedItem = action.Kind;
        _selectedKey = action.Key;
        _txtKey.Text = action.Key == Keys.None ? string.Empty : action.Key.ToString();
        _cmbMouseButton.SelectedItem = action.MouseButton;
        _chkCtrl.Checked = action.ControlModifier;
        _numActionDelay.Value = Math.Clamp(action.DelayAfterMs, (int)_numActionDelay.Minimum, (int)_numActionDelay.Maximum);
        UpdateActionInputVisibility();
    }

    private void UpdateActionInputVisibility()
    {
        bool isKeyboard = _cmbActionKind.SelectedItem is RuleActionKind.KeyboardKeyPress;
        _txtKey.Enabled = isKeyboard;
        _cmbMouseButton.Enabled = !isKeyboard;
    }

    private int SelectedActionIndex()
    {
        return _actionGrid.CurrentRow?.DataBoundItem is ActionRow row ? _actionRows.IndexOf(row) : -1;
    }

    private void SelectActionIndex(int index)
    {
        if (index < 0 || index >= _actionGrid.Rows.Count)
        {
            return;
        }

        _actionGrid.ClearSelection();
        _actionGrid.Rows[index].Selected = true;
        _actionGrid.CurrentCell = _actionGrid.Rows[index].Cells[0];
    }

    private void RefreshConditionRows()
    {
        _conditionRows.Clear();
        foreach (PixelCondition condition in _rule.Conditions)
        {
            _conditionRows.Add(new ConditionRow(condition));
        }
    }

    private void RefreshActionRows()
    {
        _actionRows.Clear();
        foreach (RuleActionDefinition action in _rule.Actions)
        {
            _actionRows.Add(new ActionRow(action));
        }
    }

    private sealed class ConditionRow
    {
        public ConditionRow(PixelCondition condition)
        {
            Condition = condition;
        }

        public PixelCondition Condition { get; }
        public int X => Condition.X;
        public int Y => Condition.Y;
        public int Tolerance
        {
            get => Condition.Tolerance;
            set => Condition.Tolerance = Math.Clamp(value, 0, 255);
        }
        public string Sample => Condition.SampleDisplay;
    }

    private sealed class ActionRow
    {
        public ActionRow(RuleActionDefinition action)
        {
            Action = action;
        }

        public RuleActionDefinition Action { get; }
        public string Kind => Action.Kind == RuleActionKind.KeyboardKeyPress ? "Keyboard" : "Mouse";
        public string Input => Action.DescribeInput();
        public string Delay => $"{Action.DelayAfterMs} ms";
    }
}
