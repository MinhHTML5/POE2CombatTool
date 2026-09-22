namespace POE2Tools
{
    partial class AutomationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            grpMacro = new System.Windows.Forms.GroupBox();
            lblPreset = new System.Windows.Forms.Label();
            cboPreset = new System.Windows.Forms.ComboBox();
            btnNew = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();
            lblMacroType = new System.Windows.Forms.Label();
            cboMacroType = new System.Windows.Forms.ComboBox();
            lblDelay = new System.Windows.Forms.Label();
            txtDelay = new System.Windows.Forms.TextBox();
            lblDelayHelp = new System.Windows.Forms.Label();
            pickupEditor = new ItemPickupEditor();
            lblHotkeys = new System.Windows.Forms.Label();
            lstActions = new System.Windows.Forms.ListView();
            colIndex = new System.Windows.Forms.ColumnHeader();
            colTime = new System.Windows.Forms.ColumnHeader();
            colDelta = new System.Windows.Forms.ColumnHeader();
            colAction = new System.Windows.Forms.ColumnHeader();
            colDetail = new System.Windows.Forms.ColumnHeader();
            btnMacroStepDelete = new System.Windows.Forms.Button();
            lblDuration = new System.Windows.Forms.Label();
            grpAction = new System.Windows.Forms.GroupBox();
            lblActionPreset = new System.Windows.Forms.Label();
            cboActionPreset = new System.Windows.Forms.ComboBox();
            btnActionNew = new System.Windows.Forms.Button();
            btnActionSave = new System.Windows.Forms.Button();
            btnActionDelete = new System.Windows.Forms.Button();
            lblActionHotkeys = new System.Windows.Forms.Label();
            lstSteps = new System.Windows.Forms.ListView();
            colStepIndex = new System.Windows.Forms.ColumnHeader();
            colStepMacro = new System.Windows.Forms.ColumnHeader();
            colStepConditions = new System.Windows.Forms.ColumnHeader();
            btnStepAdd = new System.Windows.Forms.Button();
            btnStepRemove = new System.Windows.Forms.Button();
            btnStepUp = new System.Windows.Forms.Button();
            btnStepDown = new System.Windows.Forms.Button();
            btnStepCopy = new System.Windows.Forms.Button();
            btnStepPaste = new System.Windows.Forms.Button();
            lblStepMacro = new System.Windows.Forms.Label();
            cboStepMacro = new System.Windows.Forms.ComboBox();
            lblConditions = new System.Windows.Forms.Label();
            lstConditions = new System.Windows.Forms.ListView();
            colCondition = new System.Windows.Forms.ColumnHeader();
            colConditionTarget = new System.Windows.Forms.ColumnHeader();
            btnConditionAdd = new System.Windows.Forms.Button();
            btnConditionEdit = new System.Windows.Forms.Button();
            btnConditionRemove = new System.Windows.Forms.Button();
            btnConditionCopy = new System.Windows.Forms.Button();
            btnConditionPaste = new System.Windows.Forms.Button();
            lblStepHelp = new System.Windows.Forms.Label();
            lblStatus = new System.Windows.Forms.Label();
            chkStopAfter = new System.Windows.Forms.CheckBox();
            txtStopAfter = new System.Windows.Forms.TextBox();
            lblStopAfterUnit = new System.Windows.Forms.Label();
            chkSleepWhenDone = new System.Windows.Forms.CheckBox();
            tmrStatus = new System.Windows.Forms.Timer(components);
            grpMacro.SuspendLayout();
            grpAction.SuspendLayout();
            SuspendLayout();
            // 
            // grpMacro
            // 
            grpMacro.Controls.Add(lblPreset);
            grpMacro.Controls.Add(cboPreset);
            grpMacro.Controls.Add(btnNew);
            grpMacro.Controls.Add(btnSave);
            grpMacro.Controls.Add(btnDelete);
            grpMacro.Controls.Add(lblMacroType);
            grpMacro.Controls.Add(cboMacroType);
            grpMacro.Controls.Add(lblDelay);
            grpMacro.Controls.Add(txtDelay);
            grpMacro.Controls.Add(lblDelayHelp);
            grpMacro.Controls.Add(pickupEditor);
            grpMacro.Controls.Add(lblHotkeys);
            grpMacro.Controls.Add(lstActions);
            grpMacro.Controls.Add(btnMacroStepDelete);
            grpMacro.Controls.Add(lblDuration);
            grpMacro.Location = new System.Drawing.Point(12, 8);
            grpMacro.Name = "grpMacro";
            grpMacro.Size = new System.Drawing.Size(487, 540);
            grpMacro.TabIndex = 0;
            grpMacro.TabStop = false;
            grpMacro.Text = "Macro preset";
            // 
            // lblPreset
            // 
            lblPreset.AutoSize = true;
            lblPreset.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            lblPreset.Location = new System.Drawing.Point(10, 27);
            lblPreset.Name = "lblPreset";
            lblPreset.Size = new System.Drawing.Size(100, 15);
            lblPreset.TabIndex = 1;
            lblPreset.Text = "Preset:";
            // 
            // cboPreset
            // 
            cboPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboPreset.FormattingEnabled = true;
            cboPreset.Location = new System.Drawing.Point(62, 24);
            cboPreset.Name = "cboPreset";
            cboPreset.Size = new System.Drawing.Size(187, 23);
            cboPreset.TabIndex = 2;
            cboPreset.SelectedIndexChanged += cboPreset_SelectedIndexChanged;
            // 
            // btnNew
            // 
            btnNew.Location = new System.Drawing.Point(255, 23);
            btnNew.Name = "btnNew";
            btnNew.Size = new System.Drawing.Size(70, 25);
            btnNew.TabIndex = 3;
            btnNew.Text = "New";
            btnNew.UseVisualStyleBackColor = true;
            btnNew.Click += btnNew_Click;
            // 
            // btnSave
            // 
            btnSave.Enabled = false;
            btnSave.Location = new System.Drawing.Point(331, 23);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(70, 25);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnDelete
            // 
            btnDelete.Enabled = false;
            btnDelete.Location = new System.Drawing.Point(407, 23);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(70, 25);
            btnDelete.TabIndex = 5;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // lblMacroType
            // 
            lblMacroType.AutoSize = true;
            lblMacroType.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            lblMacroType.Location = new System.Drawing.Point(10, 56);
            lblMacroType.Name = "lblMacroType";
            lblMacroType.Size = new System.Drawing.Size(100, 15);
            lblMacroType.TabIndex = 33;
            lblMacroType.Text = "Type:";
            // 
            // cboMacroType
            // 
            cboMacroType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboMacroType.FormattingEnabled = true;
            cboMacroType.Location = new System.Drawing.Point(62, 53);
            cboMacroType.Name = "cboMacroType";
            cboMacroType.Size = new System.Drawing.Size(187, 23);
            cboMacroType.TabIndex = 34;
            cboMacroType.SelectedIndexChanged += cboMacroType_SelectedIndexChanged;
            // 
            // lblDelay
            // 
            lblDelay.AutoSize = true;
            lblDelay.Location = new System.Drawing.Point(10, 88);
            lblDelay.Name = "lblDelay";
            lblDelay.Size = new System.Drawing.Size(100, 15);
            lblDelay.TabIndex = 35;
            lblDelay.Text = "Delay (ms):";
            lblDelay.Visible = false;
            // 
            // txtDelay
            // 
            txtDelay.Location = new System.Drawing.Point(82, 85);
            txtDelay.MaxLength = 8;
            txtDelay.Name = "txtDelay";
            txtDelay.Size = new System.Drawing.Size(80, 23);
            txtDelay.TabIndex = 36;
            txtDelay.Visible = false;
            txtDelay.WordWrap = false;
            txtDelay.TextChanged += txtDelay_TextChanged;
            // 
            // lblDelayHelp
            // 
            lblDelayHelp.AutoSize = true;
            lblDelayHelp.Location = new System.Drawing.Point(10, 117);
            lblDelayHelp.Name = "lblDelayHelp";
            lblDelayHelp.Size = new System.Drawing.Size(100, 15);
            lblDelayHelp.TabIndex = 37;
            lblDelayHelp.Text = "Does nothing for this long, then ends. Use it as the macro of a step in an action preset.";
            lblDelayHelp.Visible = false;
            // 
            // pickupEditor
            // 
            pickupEditor.Location = new System.Drawing.Point(10, 85);
            pickupEditor.Name = "pickupEditor";
            pickupEditor.Size = new System.Drawing.Size(467, 365);
            pickupEditor.TabIndex = 38;
            pickupEditor.Visible = false;
            // 
            // lblHotkeys
            // 
            lblHotkeys.AutoSize = true;
            lblHotkeys.Location = new System.Drawing.Point(10, 85);
            lblHotkeys.Name = "lblHotkeys";
            lblHotkeys.Size = new System.Drawing.Size(100, 15);
            lblHotkeys.TabIndex = 6;
            lblHotkeys.Text = "Ctrl + Up: start / stop recording.   Ctrl + Down: loop it, if there is no action.\r\nDouble click a time, a key or a click position to change it.";
            // 
            // lstActions
            // 
            lstActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colIndex, colTime, colDelta, colAction, colDetail });
            lstActions.FullRowSelect = true;
            lstActions.GridLines = true;
            lstActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lstActions.HideSelection = false;
            lstActions.Location = new System.Drawing.Point(10, 122);
            lstActions.MultiSelect = false;
            lstActions.Name = "lstActions";
            lstActions.Size = new System.Drawing.Size(467, 383);
            lstActions.TabIndex = 7;
            lstActions.UseCompatibleStateImageBehavior = false;
            lstActions.View = System.Windows.Forms.View.Details;
            lstActions.SelectedIndexChanged += lstActions_SelectedIndexChanged;
            lstActions.MouseDoubleClick += lstActions_MouseDoubleClick;
            // 
            // colIndex
            // 
            colIndex.Text = "#";
            colIndex.Width = 40;
            // 
            // colTime
            // 
            colTime.Text = "Time from start";
            colTime.Width = 95;
            //
            // colDelta
            //
            colDelta.Text = "Time from last step";
            colDelta.Width = 120;
            // 
            // colAction
            // 
            colAction.Text = "Action";
            colAction.Width = 85;
            // 
            // colDetail
            // 
            colDetail.Text = "Detail";
            colDetail.Width = 106;
            // 
            // btnMacroStepDelete
            // 
            btnMacroStepDelete.Enabled = false;
            btnMacroStepDelete.Location = new System.Drawing.Point(10, 509);
            btnMacroStepDelete.Name = "btnMacroStepDelete";
            btnMacroStepDelete.Size = new System.Drawing.Size(90, 25);
            btnMacroStepDelete.TabIndex = 39;
            btnMacroStepDelete.Text = "Delete step";
            btnMacroStepDelete.UseVisualStyleBackColor = true;
            btnMacroStepDelete.Click += btnMacroStepDelete_Click;
            // 
            // lblDuration
            // 
            lblDuration.Location = new System.Drawing.Point(277, 514);
            lblDuration.Name = "lblDuration";
            lblDuration.Size = new System.Drawing.Size(200, 15);
            lblDuration.TabIndex = 8;
            lblDuration.Text = "Loop length: 0 ms";
            lblDuration.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // grpAction
            // 
            grpAction.Controls.Add(lblActionPreset);
            grpAction.Controls.Add(cboActionPreset);
            grpAction.Controls.Add(btnActionNew);
            grpAction.Controls.Add(btnActionSave);
            grpAction.Controls.Add(btnActionDelete);
            grpAction.Controls.Add(lblActionHotkeys);
            grpAction.Controls.Add(lstSteps);
            grpAction.Controls.Add(btnStepAdd);
            grpAction.Controls.Add(btnStepRemove);
            grpAction.Controls.Add(btnStepUp);
            grpAction.Controls.Add(btnStepDown);
            grpAction.Controls.Add(btnStepCopy);
            grpAction.Controls.Add(btnStepPaste);
            grpAction.Controls.Add(lblStepMacro);
            grpAction.Controls.Add(cboStepMacro);
            grpAction.Controls.Add(lblConditions);
            grpAction.Controls.Add(lstConditions);
            grpAction.Controls.Add(btnConditionAdd);
            grpAction.Controls.Add(btnConditionEdit);
            grpAction.Controls.Add(btnConditionRemove);
            grpAction.Controls.Add(btnConditionCopy);
            grpAction.Controls.Add(btnConditionPaste);
            grpAction.Controls.Add(lblStepHelp);
            grpAction.Location = new System.Drawing.Point(511, 8);
            grpAction.Name = "grpAction";
            grpAction.Size = new System.Drawing.Size(487, 540);
            grpAction.TabIndex = 1;
            grpAction.TabStop = false;
            grpAction.Text = "Action preset";
            // 
            // lblActionPreset
            // 
            lblActionPreset.AutoSize = true;
            lblActionPreset.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            lblActionPreset.Location = new System.Drawing.Point(10, 27);
            lblActionPreset.Name = "lblActionPreset";
            lblActionPreset.Size = new System.Drawing.Size(100, 15);
            lblActionPreset.TabIndex = 9;
            lblActionPreset.Text = "Preset:";
            // 
            // cboActionPreset
            // 
            cboActionPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboActionPreset.FormattingEnabled = true;
            cboActionPreset.Location = new System.Drawing.Point(62, 24);
            cboActionPreset.Name = "cboActionPreset";
            cboActionPreset.Size = new System.Drawing.Size(187, 23);
            cboActionPreset.TabIndex = 10;
            cboActionPreset.SelectedIndexChanged += cboActionPreset_SelectedIndexChanged;
            // 
            // btnActionNew
            // 
            btnActionNew.Location = new System.Drawing.Point(255, 23);
            btnActionNew.Name = "btnActionNew";
            btnActionNew.Size = new System.Drawing.Size(70, 25);
            btnActionNew.TabIndex = 11;
            btnActionNew.Text = "New";
            btnActionNew.UseVisualStyleBackColor = true;
            btnActionNew.Click += btnActionNew_Click;
            // 
            // btnActionSave
            // 
            btnActionSave.Enabled = false;
            btnActionSave.Location = new System.Drawing.Point(331, 23);
            btnActionSave.Name = "btnActionSave";
            btnActionSave.Size = new System.Drawing.Size(70, 25);
            btnActionSave.TabIndex = 12;
            btnActionSave.Text = "Save";
            btnActionSave.UseVisualStyleBackColor = true;
            btnActionSave.Click += btnActionSave_Click;
            // 
            // btnActionDelete
            // 
            btnActionDelete.Enabled = false;
            btnActionDelete.Location = new System.Drawing.Point(407, 23);
            btnActionDelete.Name = "btnActionDelete";
            btnActionDelete.Size = new System.Drawing.Size(70, 25);
            btnActionDelete.TabIndex = 13;
            btnActionDelete.Text = "Delete";
            btnActionDelete.UseVisualStyleBackColor = true;
            btnActionDelete.Click += btnActionDelete_Click;
            // 
            // lblActionHotkeys
            // 
            lblActionHotkeys.AutoSize = true;
            lblActionHotkeys.Location = new System.Drawing.Point(10, 56);
            lblActionHotkeys.Name = "lblActionHotkeys";
            lblActionHotkeys.Size = new System.Drawing.Size(100, 15);
            lblActionHotkeys.TabIndex = 14;
            lblActionHotkeys.Text = "Ctrl + Down: start / stop the action preset, from step 1.";
            // 
            // lstSteps
            // 
            lstSteps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colStepIndex, colStepMacro, colStepConditions });
            lstSteps.FullRowSelect = true;
            lstSteps.GridLines = true;
            lstSteps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lstSteps.HideSelection = false;
            lstSteps.Location = new System.Drawing.Point(10, 80);
            lstSteps.MultiSelect = false;
            lstSteps.Name = "lstSteps";
            lstSteps.Size = new System.Drawing.Size(375, 180);
            lstSteps.TabIndex = 15;
            lstSteps.UseCompatibleStateImageBehavior = false;
            lstSteps.View = System.Windows.Forms.View.Details;
            lstSteps.SelectedIndexChanged += lstSteps_SelectedIndexChanged;
            // 
            // colStepIndex
            // 
            colStepIndex.Text = "Step";
            colStepIndex.Width = 40;
            // 
            // colStepMacro
            // 
            colStepMacro.Text = "Macro preset";
            colStepMacro.Width = 140;
            // 
            // colStepConditions
            // 
            colStepConditions.Text = "Conditions";
            colStepConditions.Width = 190;
            // 
            // btnStepAdd
            // 
            btnStepAdd.Location = new System.Drawing.Point(391, 80);
            btnStepAdd.Name = "btnStepAdd";
            btnStepAdd.Size = new System.Drawing.Size(86, 25);
            btnStepAdd.TabIndex = 16;
            btnStepAdd.Text = "Add step";
            btnStepAdd.UseVisualStyleBackColor = true;
            btnStepAdd.Click += btnStepAdd_Click;
            // 
            // btnStepRemove
            // 
            btnStepRemove.Location = new System.Drawing.Point(391, 110);
            btnStepRemove.Name = "btnStepRemove";
            btnStepRemove.Size = new System.Drawing.Size(86, 25);
            btnStepRemove.TabIndex = 17;
            btnStepRemove.Text = "Remove";
            btnStepRemove.UseVisualStyleBackColor = true;
            btnStepRemove.Click += btnStepRemove_Click;
            // 
            // btnStepUp
            // 
            btnStepUp.Location = new System.Drawing.Point(391, 140);
            btnStepUp.Name = "btnStepUp";
            btnStepUp.Size = new System.Drawing.Size(86, 25);
            btnStepUp.TabIndex = 18;
            btnStepUp.Text = "Move up";
            btnStepUp.UseVisualStyleBackColor = true;
            btnStepUp.Click += btnStepUp_Click;
            // 
            // btnStepDown
            // 
            btnStepDown.Location = new System.Drawing.Point(391, 170);
            btnStepDown.Name = "btnStepDown";
            btnStepDown.Size = new System.Drawing.Size(86, 25);
            btnStepDown.TabIndex = 19;
            btnStepDown.Text = "Move down";
            btnStepDown.UseVisualStyleBackColor = true;
            btnStepDown.Click += btnStepDown_Click;
            // 
            // btnStepCopy
            // 
            btnStepCopy.Location = new System.Drawing.Point(391, 205);
            btnStepCopy.Name = "btnStepCopy";
            btnStepCopy.Size = new System.Drawing.Size(86, 25);
            btnStepCopy.TabIndex = 42;
            btnStepCopy.Text = "Copy";
            btnStepCopy.UseVisualStyleBackColor = true;
            btnStepCopy.Click += btnStepCopy_Click;
            // 
            // btnStepPaste
            // 
            btnStepPaste.Location = new System.Drawing.Point(391, 235);
            btnStepPaste.Name = "btnStepPaste";
            btnStepPaste.Size = new System.Drawing.Size(86, 25);
            btnStepPaste.TabIndex = 43;
            btnStepPaste.Text = "Paste";
            btnStepPaste.UseVisualStyleBackColor = true;
            btnStepPaste.Click += btnStepPaste_Click;
            // 
            // lblStepMacro
            // 
            lblStepMacro.AutoSize = true;
            lblStepMacro.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            lblStepMacro.Location = new System.Drawing.Point(10, 273);
            lblStepMacro.Name = "lblStepMacro";
            lblStepMacro.Size = new System.Drawing.Size(100, 15);
            lblStepMacro.TabIndex = 20;
            lblStepMacro.Text = "Macro preset of the step:";
            // 
            // cboStepMacro
            // 
            cboStepMacro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboStepMacro.FormattingEnabled = true;
            cboStepMacro.Location = new System.Drawing.Point(170, 270);
            cboStepMacro.Name = "cboStepMacro";
            cboStepMacro.Size = new System.Drawing.Size(215, 23);
            cboStepMacro.TabIndex = 21;
            cboStepMacro.SelectedIndexChanged += cboStepMacro_SelectedIndexChanged;
            // 
            // lblConditions
            // 
            lblConditions.AutoSize = true;
            lblConditions.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            lblConditions.Location = new System.Drawing.Point(10, 302);
            lblConditions.Name = "lblConditions";
            lblConditions.Size = new System.Drawing.Size(100, 15);
            lblConditions.TabIndex = 22;
            lblConditions.Text = "Conditions of the step, checked from top to bottom:";
            // 
            // lstConditions
            // 
            lstConditions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colCondition, colConditionTarget });
            lstConditions.FullRowSelect = true;
            lstConditions.GridLines = true;
            lstConditions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lstConditions.HideSelection = false;
            lstConditions.Location = new System.Drawing.Point(10, 322);
            lstConditions.MultiSelect = false;
            lstConditions.Name = "lstConditions";
            lstConditions.Size = new System.Drawing.Size(375, 183);
            lstConditions.TabIndex = 23;
            lstConditions.UseCompatibleStateImageBehavior = false;
            lstConditions.View = System.Windows.Forms.View.Details;
            lstConditions.SelectedIndexChanged += lstConditions_SelectedIndexChanged;
            lstConditions.DoubleClick += lstConditions_DoubleClick;
            // 
            // colCondition
            // 
            colCondition.Text = "When";
            colCondition.Width = 245;
            // 
            // colConditionTarget
            // 
            colConditionTarget.Text = "Then go to";
            colConditionTarget.Width = 125;
            // 
            // btnConditionAdd
            // 
            btnConditionAdd.Location = new System.Drawing.Point(391, 322);
            btnConditionAdd.Name = "btnConditionAdd";
            btnConditionAdd.Size = new System.Drawing.Size(86, 25);
            btnConditionAdd.TabIndex = 24;
            btnConditionAdd.Text = "Add...";
            btnConditionAdd.UseVisualStyleBackColor = true;
            btnConditionAdd.Click += btnConditionAdd_Click;
            // 
            // btnConditionEdit
            // 
            btnConditionEdit.Location = new System.Drawing.Point(391, 352);
            btnConditionEdit.Name = "btnConditionEdit";
            btnConditionEdit.Size = new System.Drawing.Size(86, 25);
            btnConditionEdit.TabIndex = 25;
            btnConditionEdit.Text = "Edit...";
            btnConditionEdit.UseVisualStyleBackColor = true;
            btnConditionEdit.Click += btnConditionEdit_Click;
            // 
            // btnConditionRemove
            // 
            btnConditionRemove.Location = new System.Drawing.Point(391, 382);
            btnConditionRemove.Name = "btnConditionRemove";
            btnConditionRemove.Size = new System.Drawing.Size(86, 25);
            btnConditionRemove.TabIndex = 26;
            btnConditionRemove.Text = "Remove";
            btnConditionRemove.UseVisualStyleBackColor = true;
            btnConditionRemove.Click += btnConditionRemove_Click;
            // 
            // btnConditionCopy
            // 
            btnConditionCopy.Location = new System.Drawing.Point(391, 417);
            btnConditionCopy.Name = "btnConditionCopy";
            btnConditionCopy.Size = new System.Drawing.Size(86, 25);
            btnConditionCopy.TabIndex = 40;
            btnConditionCopy.Text = "Copy";
            btnConditionCopy.UseVisualStyleBackColor = true;
            btnConditionCopy.Click += btnConditionCopy_Click;
            // 
            // btnConditionPaste
            // 
            btnConditionPaste.Location = new System.Drawing.Point(391, 447);
            btnConditionPaste.Name = "btnConditionPaste";
            btnConditionPaste.Size = new System.Drawing.Size(86, 25);
            btnConditionPaste.TabIndex = 41;
            btnConditionPaste.Text = "Paste";
            btnConditionPaste.UseVisualStyleBackColor = true;
            btnConditionPaste.Click += btnConditionPaste_Click;
            // 
            // lblStepHelp
            // 
            lblStepHelp.AutoSize = true;
            lblStepHelp.Location = new System.Drawing.Point(10, 514);
            lblStepHelp.Name = "lblStepHelp";
            lblStepHelp.Size = new System.Drawing.Size(100, 15);
            lblStepHelp.TabIndex = 27;
            lblStepHelp.Text = "A step loops its macro until one of its conditions is met.";
            // 
            // lblStatus
            // 
            lblStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(12, 554);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(700, 50);
            lblStatus.TabIndex = 28;
            lblStatus.Text = "Status";
            //
            // chkStopAfter
            //
            chkStopAfter.AutoSize = true;
            chkStopAfter.Location = new System.Drawing.Point(738, 557);
            chkStopAfter.Name = "chkStopAfter";
            chkStopAfter.Size = new System.Drawing.Size(80, 19);
            chkStopAfter.TabIndex = 29;
            chkStopAfter.Text = "Stop after:";
            chkStopAfter.UseVisualStyleBackColor = true;
            chkStopAfter.CheckedChanged += chkStopAfter_CheckedChanged;
            //
            // txtStopAfter
            //
            txtStopAfter.Location = new System.Drawing.Point(824, 555);
            txtStopAfter.MaxLength = 6;
            txtStopAfter.Name = "txtStopAfter";
            txtStopAfter.Size = new System.Drawing.Size(60, 23);
            txtStopAfter.TabIndex = 30;
            txtStopAfter.Text = "10";
            txtStopAfter.WordWrap = false;
            txtStopAfter.TextChanged += txtStopAfter_TextChanged;
            //
            // lblStopAfterUnit
            //
            lblStopAfterUnit.AutoSize = true;
            lblStopAfterUnit.Location = new System.Drawing.Point(890, 558);
            lblStopAfterUnit.Name = "lblStopAfterUnit";
            lblStopAfterUnit.Size = new System.Drawing.Size(108, 15);
            lblStopAfterUnit.TabIndex = 31;
            lblStopAfterUnit.Text = "times of final step";
            //
            // chkSleepWhenDone
            //
            chkSleepWhenDone.AutoSize = true;
            chkSleepWhenDone.Location = new System.Drawing.Point(738, 583);
            chkSleepWhenDone.Name = "chkSleepWhenDone";
            chkSleepWhenDone.Size = new System.Drawing.Size(115, 19);
            chkSleepWhenDone.TabIndex = 32;
            chkSleepWhenDone.Text = "Sleep when done";
            chkSleepWhenDone.UseVisualStyleBackColor = true;
            chkSleepWhenDone.CheckedChanged += chkSleepWhenDone_CheckedChanged;
            // 
            // tmrStatus
            // 
            tmrStatus.Interval = 100;
            tmrStatus.Tick += tmrStatus_Tick;
            // 
            // AutomationForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1010, 612);
            Controls.Add(grpMacro);
            Controls.Add(grpAction);
            Controls.Add(chkSleepWhenDone);
            Controls.Add(lblStopAfterUnit);
            Controls.Add(txtStopAfter);
            Controls.Add(chkStopAfter);
            Controls.Add(lblStatus);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AutomationForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Setup automation";
            FormClosing += AutomationForm_FormClosing;
            FormClosed += AutomationForm_FormClosed;
            Load += AutomationForm_Load;
            grpMacro.ResumeLayout(false);
            grpMacro.PerformLayout();
            grpAction.ResumeLayout(false);
            grpAction.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpMacro;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.ComboBox cboPreset;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblMacroType;
        private System.Windows.Forms.ComboBox cboMacroType;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.TextBox txtDelay;
        private System.Windows.Forms.Label lblDelayHelp;
        private ItemPickupEditor pickupEditor;
        private System.Windows.Forms.Label lblHotkeys;
        private System.Windows.Forms.ListView lstActions;
        private System.Windows.Forms.ColumnHeader colIndex;
        private System.Windows.Forms.ColumnHeader colTime;
        private System.Windows.Forms.ColumnHeader colDelta;
        private System.Windows.Forms.ColumnHeader colAction;
        private System.Windows.Forms.ColumnHeader colDetail;
        private System.Windows.Forms.Button btnMacroStepDelete;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.GroupBox grpAction;
        private System.Windows.Forms.Label lblActionPreset;
        private System.Windows.Forms.ComboBox cboActionPreset;
        private System.Windows.Forms.Button btnActionNew;
        private System.Windows.Forms.Button btnActionSave;
        private System.Windows.Forms.Button btnActionDelete;
        private System.Windows.Forms.Label lblActionHotkeys;
        private System.Windows.Forms.ListView lstSteps;
        private System.Windows.Forms.ColumnHeader colStepIndex;
        private System.Windows.Forms.ColumnHeader colStepMacro;
        private System.Windows.Forms.ColumnHeader colStepConditions;
        private System.Windows.Forms.Button btnStepAdd;
        private System.Windows.Forms.Button btnStepRemove;
        private System.Windows.Forms.Button btnStepUp;
        private System.Windows.Forms.Button btnStepDown;
        private System.Windows.Forms.Button btnStepCopy;
        private System.Windows.Forms.Button btnStepPaste;
        private System.Windows.Forms.Label lblStepMacro;
        private System.Windows.Forms.ComboBox cboStepMacro;
        private System.Windows.Forms.Label lblConditions;
        private System.Windows.Forms.ListView lstConditions;
        private System.Windows.Forms.ColumnHeader colCondition;
        private System.Windows.Forms.ColumnHeader colConditionTarget;
        private System.Windows.Forms.Button btnConditionAdd;
        private System.Windows.Forms.Button btnConditionEdit;
        private System.Windows.Forms.Button btnConditionRemove;
        private System.Windows.Forms.Button btnConditionCopy;
        private System.Windows.Forms.Button btnConditionPaste;
        private System.Windows.Forms.Label lblStepHelp;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkStopAfter;
        private System.Windows.Forms.TextBox txtStopAfter;
        private System.Windows.Forms.Label lblStopAfterUnit;
        private System.Windows.Forms.CheckBox chkSleepWhenDone;
        private System.Windows.Forms.Timer tmrStatus;
    }
}
