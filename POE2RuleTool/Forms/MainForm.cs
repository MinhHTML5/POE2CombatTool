using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using POE2RuleTool.Models;
using POE2RuleTool.Modules;
using POE2RuleTool.Services;
using POE2Tools.Utilities;

namespace POE2RuleTool.Forms;

public sealed class MainForm : Form
{
    private const int WM_INPUT = 0x00FF;
    private const string SampledColumnName = "SampledColumn";
    private const int SampleSwatchSize = 20;
    private const int SampleSwatchGap = 5;
    private const int LoadingCornerInset = 5;
    private const int LoadingBlackThreshold = 10;
    private readonly ColorUtil _colorUtil = new();
    private readonly InputHook _inputHook = new();
    private readonly WindowsUtil _windowsUtil = new();
    private readonly RuleRunner _runner;
    private readonly SprintModule _sprintModule;
    private readonly List<RuleDefinition> _rules = new();
    private readonly BindingList<RuleRow> _ruleRows = new();
    private readonly DataGridView _ruleGrid = new();
    private readonly CheckBox _chkSmartSprint = new() { Text = "Smart sprint", AutoSize = true };
    private Button _btnStartStop = null!;
    private readonly Label _statusLabel = new();
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 40 };
    private readonly System.Diagnostics.Stopwatch _stopwatch = new();
    private bool _rawInputRegistered;
    private bool _loadingScreenPaused;

    private string? _currentFilePath;

    public MainForm()
    {
        _runner = new RuleRunner(_colorUtil, _inputHook);
        _sprintModule = new SprintModule(_inputHook);
        _runner.StatusChanged += SetStatus;

        InitializeComponent();
        TryLoadLastConfig();
        RefreshRuleRows();

        _timer.Tick += Timer_Tick;
        _stopwatch.Start();
        _timer.Start();
    }

    private void InitializeComponent()
    {
        Text = "POE2 Rule Tool";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1040, 620);
        MinimumSize = new Size(900, 520);

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(8),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        toolbar.Controls.Add(CreateButton("SAMPLE", SampleRules_Click));
        _btnStartStop = CreateButton("START", BtnStartStop_Click);
        toolbar.Controls.Add(_btnStartStop);
        toolbar.Controls.Add(CreateButton("Add Rule", AddRule_Click));
        toolbar.Controls.Add(CreateButton("Edit Rule", EditSelectedRule_Click));
        toolbar.Controls.Add(CreateButton("Delete Rule", DeleteSelectedRule_Click));
        toolbar.Controls.Add(CreateButton("Save JSON", SaveRules_Click));
        toolbar.Controls.Add(CreateButton("Load JSON", LoadRules_Click));

        _ruleGrid.Dock = DockStyle.Fill;
        _ruleGrid.AutoGenerateColumns = false;
        _ruleGrid.AllowUserToAddRows = false;
        _ruleGrid.AllowUserToDeleteRows = false;
        _ruleGrid.ReadOnly = true;
        _ruleGrid.MultiSelect = false;
        _ruleGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _ruleGrid.RowHeadersVisible = false;
        _ruleGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _ruleGrid.DataSource = _ruleRows;
        _ruleGrid.RowTemplate.Height = 30;
        _ruleGrid.CellDoubleClick += RuleGrid_CellDoubleClick;
        _ruleGrid.CellPainting += RuleGrid_CellPainting;
        _chkSmartSprint.CheckedChanged += ChkSmartSprint_CheckedChanged;

        _ruleGrid.Columns.Add(new DataGridViewCheckBoxColumn
        {
            HeaderText = "On",
            DataPropertyName = nameof(RuleRow.Enabled),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Width = 42
        });
        _ruleGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Name",
            DataPropertyName = nameof(RuleRow.Name),
            FillWeight = 120
        });
        _ruleGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Mode",
            DataPropertyName = nameof(RuleRow.Mode),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Width = 70
        });
        _ruleGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Points",
            DataPropertyName = nameof(RuleRow.ConditionCount),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Width = 70
        });
        _ruleGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = SampledColumnName,
            HeaderText = "Sampled",
            DataPropertyName = nameof(RuleRow.Sampled),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Width = 150
        });
        _ruleGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "First Action",
            DataPropertyName = nameof(RuleRow.FirstAction),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Width = 180
        });
        _ruleGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Cooldown",
            DataPropertyName = nameof(RuleRow.Cooldown),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Width = 95
        });

        _statusLabel.Dock = DockStyle.Bottom;
        _statusLabel.Height = 28;
        _statusLabel.BorderStyle = BorderStyle.Fixed3D;
        _statusLabel.Padding = new Padding(8, 0, 0, 0);
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _statusLabel.Text = "Ready.";

        var contentLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contentLayout.Controls.Add(_ruleGrid, 0, 0);
        contentLayout.Controls.Add(BuildCustomModulePanel(), 1, 0);

        Controls.Add(contentLayout);
        Controls.Add(_statusLabel);
        Controls.Add(toolbar);
    }


    private Control BuildCustomModulePanel()
    {
        var group = new GroupBox
        {
            Text = "Custom module",
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };

        layout.Controls.Add(_chkSmartSprint);
        layout.Controls.Add(new Label
        {
            Text = "",
            AutoSize = false,
            Height = 320,
            Width = 180
        });

        group.Controls.Add(layout);
        return group;
    }
    private static Button CreateButton(string text, EventHandler clickHandler)
    {
        var button = new Button
        {
            Text = text,
            Width = 110,
            Height = 30,
            Margin = new Padding(0, 0, 8, 0)
        };
        button.Click += clickHandler;
        return button;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_rawInputRegistered)
        {
            return;
        }

        _inputHook.RegisterRawInputDevices(Handle, OnMouseKeyEvent, OnKeyEvent);
        _rawInputRegistered = true;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_INPUT)
        {
            _inputHook.ProcessRawInput(m.LParam);
        }

        base.WndProc(ref m);
    }

    private void OnKeyEvent(Keys key, bool isDown, bool isControlDown)
    {
        if (key == Keys.B && !isDown && isControlDown)
        {
            ToggleRunner();
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
    }
    private void Timer_Tick(object? sender, EventArgs e)
    {
        int deltaTime = (int)_stopwatch.Elapsed.TotalMilliseconds;
        _stopwatch.Restart();

        if (_runner.IsRunning && IsLoadingScreenDetected())
        {
            if (!_loadingScreenPaused)
            {
                _loadingScreenPaused = true;
                SetStatus("Pausing, loading screen detected...");
            }

            _sprintModule.MainLoop(deltaTime, false, _runner.IsRunning);
            return;
        }

        if (_loadingScreenPaused)
        {
            _loadingScreenPaused = false;
            SetStatus("Loading screen cleared. Resuming...");
        }

        _sprintModule.MainLoop(deltaTime, true, _runner.IsRunning);
        _runner.Tick(_rules);
    }
    private bool IsLoadingScreenDetected()
    {
        Rectangle bounds = Screen.PrimaryScreen?.Bounds ?? SystemInformation.VirtualScreen;
        var bottomLeft = new Point(bounds.Left + LoadingCornerInset, bounds.Bottom - LoadingCornerInset);
        var bottomRight = new Point(bounds.Right - LoadingCornerInset, bounds.Bottom - LoadingCornerInset);

        Color leftColor = _colorUtil.GetColorAt(bottomLeft);
        Color rightColor = _colorUtil.GetColorAt(bottomRight);

        return IsBelowBlackThreshold(leftColor) && IsBelowBlackThreshold(rightColor);
    }

    private static bool IsBelowBlackThreshold(Color color)
    {
        return color.R < LoadingBlackThreshold
            && color.G < LoadingBlackThreshold
            && color.B < LoadingBlackThreshold;
    }


    private void ChkSmartSprint_CheckedChanged(object? sender, EventArgs e)
    {
        _sprintModule.SetResponsiveDodge(_chkSmartSprint.Checked);
    }
    private void BtnStartStop_Click(object? sender, EventArgs e)
    {
        ToggleRunner();
    }

    private void RuleGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        EditSelectedRule();
    }

    private void RuleGrid_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || _ruleGrid.Columns[e.ColumnIndex].Name != SampledColumnName)
        {
            return;
        }

        e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.SelectionBackground);
        Graphics? graphics = e.Graphics;
        if (graphics == null)
        {
            e.Handled = true;
            return;
        }


        if (_ruleGrid.Rows[e.RowIndex].DataBoundItem is not RuleRow row)
        {
            e.Handled = true;
            return;
        }

        int x = e.CellBounds.Left + 6;
        int y = e.CellBounds.Top + Math.Max(0, (e.CellBounds.Height - SampleSwatchSize) / 2);

        foreach (PixelCondition condition in row.Rule.Conditions.Take(5))
        {
            var rect = new Rectangle(x, y, SampleSwatchSize, SampleSwatchSize);
            Color fillColor = condition.HasSample ? condition.SampleColor : SystemColors.ControlLight;

            using (var brush = new SolidBrush(fillColor))
            {
                graphics.FillRectangle(brush, rect);
            }

            graphics.DrawRectangle(Pens.DimGray, rect);

            if (!condition.HasSample)
            {
                graphics.DrawLine(Pens.Gray, rect.Left + 3, rect.Bottom - 4, rect.Right - 4, rect.Top + 3);
            }

            x += SampleSwatchSize + SampleSwatchGap;
        }

        e.Handled = true;
    }

    private void SampleRules_Click(object? sender, EventArgs e)
    {
        SampleRules();
    }

    private void AddRule_Click(object? sender, EventArgs e)
    {
        AddRule();
    }

    private void EditSelectedRule_Click(object? sender, EventArgs e)
    {
        EditSelectedRule();
    }

    private void DeleteSelectedRule_Click(object? sender, EventArgs e)
    {
        DeleteSelectedRule();
    }

    private void SaveRules_Click(object? sender, EventArgs e)
    {
        SaveRules();
    }

    private void LoadRules_Click(object? sender, EventArgs e)
    {
        LoadRules();
    }

    private void AddRule()
    {
        using var editor = new RuleEditorForm(null);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            _rules.Add(editor.EditedRule);
            RefreshRuleRows();
            SetStatus($"Added rule: {editor.EditedRule.Name}");
        }
    }

    private void EditSelectedRule()
    {
        RuleDefinition? selectedRule = GetSelectedRule();
        if (selectedRule == null)
        {
            return;
        }

        using var editor = new RuleEditorForm(selectedRule);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            int index = _rules.FindIndex(rule => rule.Id == selectedRule.Id);
            if (index >= 0)
            {
                _rules[index] = editor.EditedRule;
                RefreshRuleRows();
                SelectRule(editor.EditedRule.Id);
                SetStatus($"Updated rule: {editor.EditedRule.Name}");
            }
        }
    }

    private void DeleteSelectedRule()
    {
        RuleDefinition? selectedRule = GetSelectedRule();
        if (selectedRule == null)
        {
            return;
        }

        DialogResult result = MessageBox.Show(
            this,
            $"Delete rule '{selectedRule.Name}'?",
            "Delete Rule",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        _rules.Remove(selectedRule);
        RefreshRuleRows();
        SetStatus($"Deleted rule: {selectedRule.Name}");
    }

    private void SampleRules()
    {
        int sampled = RuleSampler.SampleRules(_rules, _colorUtil);
        RefreshRuleRows();
        SetStatus(sampled == 0 ? "No registered pixels to sample." : $"Sampled {sampled} pixel point(s).");
    }

    private void ToggleRunner()
    {
        if (_runner.IsRunning)
        {
            _runner.Stop();
            _sprintModule.Stop();
            _btnStartStop.Text = "START";
            _loadingScreenPaused = false;
            _windowsUtil.SetStarted(false);
            return;
        }

        int sampled = RuleSampler.SampleMissingRules(_rules, _colorUtil);
        if (sampled > 0)
        {
            RefreshRuleRows();
        }

        _runner.Start(_rules);
        _sprintModule.Start();
        _btnStartStop.Text = "STOP";
        _windowsUtil.SetStarted(true);

        if (sampled > 0)
        {
            SetStatus($"Sampled {sampled} missing pixel point(s), then started.");
        }
    }

    private void SaveRules()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            DefaultExt = "json",
            FileName = string.IsNullOrWhiteSpace(_currentFilePath) ? "poe2-rules.json" : Path.GetFileName(_currentFilePath)
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            RuleFileService.Save(dialog.FileName, CreateConfig());
            _currentFilePath = dialog.FileName;
            AppSettingsService.SaveLastConfigPath(dialog.FileName);
            SetStatus($"Saved rules to {dialog.FileName}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadRules()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            DefaultExt = "json"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            if (_runner.IsRunning)
            {
                _runner.Stop();
                _sprintModule.Stop();
                _btnStartStop.Text = "START";
                _windowsUtil.SetStarted(false);
            }

            ApplyConfig(RuleFileService.Load(dialog.FileName));
            _currentFilePath = dialog.FileName;
            AppSettingsService.SaveLastConfigPath(dialog.FileName);
            SetStatus($"Loaded {_rules.Count} rule(s) from {dialog.FileName}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TryLoadLastConfig()
    {
        string? path = AppSettingsService.LoadLastConfigPath();
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return;
        }

        try
        {
            ApplyConfig(RuleFileService.Load(path));
            _currentFilePath = path;
            SetStatus($"Loaded last config: {path}");
        }
        catch (Exception ex)
        {
            SetStatus($"Could not load last config: {ex.Message}");
        }
    }
    private RuleToolConfig CreateConfig()
    {
        return new RuleToolConfig
        {
            Rules = _rules,
            Modules = new CustomModuleConfig
            {
                SmartSprintEnabled = _chkSmartSprint.Checked
            }
        };
    }

    private void ApplyConfig(RuleToolConfig config)
    {
        _rules.Clear();
        _rules.AddRange(config.Rules ?? new List<RuleDefinition>());

        bool smartSprintEnabled = config.Modules?.SmartSprintEnabled ?? false;
        if (_chkSmartSprint.Checked != smartSprintEnabled)
        {
            _chkSmartSprint.Checked = smartSprintEnabled;
        }
        else
        {
            _sprintModule.SetResponsiveDodge(smartSprintEnabled);
        }

        RefreshRuleRows();
    }
    private RuleDefinition? GetSelectedRule()
    {
        return _ruleGrid.CurrentRow?.DataBoundItem is RuleRow row ? row.Rule : null;
    }

    private void SelectRule(Guid id)
    {
        for (int i = 0; i < _ruleRows.Count; i++)
        {
            if (_ruleRows[i].Rule.Id == id)
            {
                _ruleGrid.ClearSelection();
                _ruleGrid.Rows[i].Selected = true;
                _ruleGrid.CurrentCell = _ruleGrid.Rows[i].Cells[0];
                return;
            }
        }
    }

    private void RefreshRuleRows()
    {
        _ruleRows.Clear();
        foreach (RuleDefinition rule in _rules)
        {
            _ruleRows.Add(new RuleRow(rule));
        }
    }

    private void SetStatus(string message)
    {
        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => SetStatus(message)));
            return;
        }

        _statusLabel.Text = $"{DateTime.Now:T}  {message}";
        _ruleGrid.Refresh();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timer.Stop();
        _runner.Stop();
        _sprintModule.Stop();
        _loadingScreenPaused = false;
        _windowsUtil.SetStarted(false);
        base.OnFormClosed(e);
    }

    private sealed class RuleRow
    {
        public RuleRow(RuleDefinition rule)
        {
            Rule = rule;
        }

        public RuleDefinition Rule { get; }
        public bool Enabled => Rule.Enabled;
        public string Name => Rule.Name;
        public string Mode => Rule.ConditionMode.ToString().ToUpperInvariant();
        public int ConditionCount => Rule.Conditions.Count;
        public string Sampled => string.Empty;
        public string FirstAction => DescribeFirstAction();
        public string Cooldown => $"{Rule.CooldownMs} ms";
        private string DescribeFirstAction()
        {
            RuleActionDefinition? action = Rule.Actions.FirstOrDefault();
            if (action == null)
            {
                return string.Empty;
            }

            string kind = action.Kind == RuleActionKind.KeyboardKeyPress ? "Key" : "Mouse";
            string delay = action.DelayAfterMs > 0 ? $", delay {action.DelayAfterMs} ms" : string.Empty;
            return $"{kind}: {action.DescribeInput()}{delay}";
        }
    }
}
