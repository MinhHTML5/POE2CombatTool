namespace POE2Tools
{
    partial class ConditionForm
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
            lblType = new System.Windows.Forms.Label();
            cboType = new System.Windows.Forms.ComboBox();
            lblValue = new System.Windows.Forms.Label();
            numValue = new System.Windows.Forms.NumericUpDown();
            lblValueUnit = new System.Windows.Forms.Label();
            grpScreen = new System.Windows.Forms.GroupBox();
            lblTestResult = new System.Windows.Forms.Label();
            btnTest = new System.Windows.Forms.Button();
            btnCapture = new System.Windows.Forms.Button();
            lblRegion = new System.Windows.Forms.Label();
            picSample = new System.Windows.Forms.PictureBox();
            lblSample = new System.Windows.Forms.Label();
            lblPercentUnit = new System.Windows.Forms.Label();
            numPercent = new System.Windows.Forms.NumericUpDown();
            lblPercent = new System.Windows.Forms.Label();
            lblToleranceUnit = new System.Windows.Forms.Label();
            numTolerance = new System.Windows.Forms.NumericUpDown();
            lblTolerance = new System.Windows.Forms.Label();
            lblIntervalUnit = new System.Windows.Forms.Label();
            numInterval = new System.Windows.Forms.NumericUpDown();
            lblInterval = new System.Windows.Forms.Label();
            lblTarget = new System.Windows.Forms.Label();
            cboTarget = new System.Windows.Forms.ComboBox();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)numValue).BeginInit();
            grpScreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picSample).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPercent).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTolerance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numInterval).BeginInit();
            SuspendLayout();
            //
            // lblType
            //
            lblType.AutoSize = true;
            lblType.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            lblType.Location = new System.Drawing.Point(12, 15);
            lblType.Name = "lblType";
            lblType.Size = new System.Drawing.Size(41, 15);
            lblType.TabIndex = 0;
            lblType.Text = "When:";
            //
            // cboType
            //
            cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboType.FormattingEnabled = true;
            cboType.Items.AddRange(new object[] { "The macro preset of this step ended", "An area of the screen matches a sample", "The macro preset has looped for a number of times", "The macro preset has run for a number of seconds" });
            cboType.Location = new System.Drawing.Point(90, 12);
            cboType.Name = "cboType";
            cboType.Size = new System.Drawing.Size(342, 23);
            cboType.TabIndex = 1;
            cboType.SelectedIndexChanged += cboType_SelectedIndexChanged;
            //
            // lblValue
            //
            lblValue.AutoSize = true;
            lblValue.Location = new System.Drawing.Point(22, 49);
            lblValue.Name = "lblValue";
            lblValue.Size = new System.Drawing.Size(100, 15);
            lblValue.TabIndex = 7;
            lblValue.Text = "Number of loops:";
            //
            // numValue
            //
            numValue.Location = new System.Drawing.Point(192, 46);
            numValue.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numValue.Name = "numValue";
            numValue.Size = new System.Drawing.Size(80, 23);
            numValue.TabIndex = 8;
            numValue.Value = new decimal(new int[] { 5, 0, 0, 0 });
            //
            // lblValueUnit
            //
            lblValueUnit.AutoSize = true;
            lblValueUnit.Location = new System.Drawing.Point(278, 49);
            lblValueUnit.Name = "lblValueUnit";
            lblValueUnit.Size = new System.Drawing.Size(36, 15);
            lblValueUnit.TabIndex = 9;
            lblValueUnit.Text = "times";
            //
            // grpScreen
            //
            grpScreen.Controls.Add(lblTestResult);
            grpScreen.Controls.Add(btnTest);
            grpScreen.Controls.Add(btnCapture);
            grpScreen.Controls.Add(lblRegion);
            grpScreen.Controls.Add(picSample);
            grpScreen.Controls.Add(lblSample);
            grpScreen.Controls.Add(lblPercentUnit);
            grpScreen.Controls.Add(numPercent);
            grpScreen.Controls.Add(lblPercent);
            grpScreen.Controls.Add(lblToleranceUnit);
            grpScreen.Controls.Add(numTolerance);
            grpScreen.Controls.Add(lblTolerance);
            grpScreen.Controls.Add(lblIntervalUnit);
            grpScreen.Controls.Add(numInterval);
            grpScreen.Controls.Add(lblInterval);
            grpScreen.Location = new System.Drawing.Point(12, 78);
            grpScreen.Name = "grpScreen";
            grpScreen.Size = new System.Drawing.Size(420, 318);
            grpScreen.TabIndex = 2;
            grpScreen.TabStop = false;
            grpScreen.Text = "Screen capture";
            //
            // lblTestResult
            //
            lblTestResult.Location = new System.Drawing.Point(218, 287);
            lblTestResult.Name = "lblTestResult";
            lblTestResult.Size = new System.Drawing.Size(192, 15);
            lblTestResult.TabIndex = 14;
            //
            // btnTest
            //
            btnTest.Location = new System.Drawing.Point(122, 282);
            btnTest.Name = "btnTest";
            btnTest.Size = new System.Drawing.Size(90, 25);
            btnTest.TabIndex = 13;
            btnTest.Text = "Test now";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            //
            // btnCapture
            //
            btnCapture.Location = new System.Drawing.Point(10, 282);
            btnCapture.Name = "btnCapture";
            btnCapture.Size = new System.Drawing.Size(106, 25);
            btnCapture.TabIndex = 12;
            btnCapture.Text = "Capture sample...";
            btnCapture.UseVisualStyleBackColor = true;
            btnCapture.Click += btnCapture_Click;
            //
            // lblRegion
            //
            lblRegion.Location = new System.Drawing.Point(122, 112);
            lblRegion.Name = "lblRegion";
            lblRegion.Size = new System.Drawing.Size(288, 15);
            lblRegion.TabIndex = 11;
            lblRegion.Text = "No sample yet";
            lblRegion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            // picSample
            //
            picSample.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
            picSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            picSample.Location = new System.Drawing.Point(10, 132);
            picSample.Name = "picSample";
            picSample.Size = new System.Drawing.Size(400, 142);
            picSample.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            picSample.TabIndex = 10;
            picSample.TabStop = false;
            //
            // lblSample
            //
            lblSample.AutoSize = true;
            lblSample.Location = new System.Drawing.Point(10, 112);
            lblSample.Name = "lblSample";
            lblSample.Size = new System.Drawing.Size(49, 15);
            lblSample.TabIndex = 9;
            lblSample.Text = "Sample:";
            //
            // lblPercentUnit
            //
            lblPercentUnit.AutoSize = true;
            lblPercentUnit.Location = new System.Drawing.Point(266, 83);
            lblPercentUnit.Name = "lblPercentUnit";
            lblPercentUnit.Size = new System.Drawing.Size(98, 15);
            lblPercentUnit.TabIndex = 8;
            lblPercentUnit.Text = "% of the pixels";
            //
            // numPercent
            //
            numPercent.Location = new System.Drawing.Point(180, 80);
            numPercent.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numPercent.Name = "numPercent";
            numPercent.Size = new System.Drawing.Size(80, 23);
            numPercent.TabIndex = 7;
            numPercent.Value = new decimal(new int[] { 90, 0, 0, 0 });
            //
            // lblPercent
            //
            lblPercent.AutoSize = true;
            lblPercent.Location = new System.Drawing.Point(10, 83);
            lblPercent.Name = "lblPercent";
            lblPercent.Size = new System.Drawing.Size(115, 15);
            lblPercent.TabIndex = 6;
            lblPercent.Text = "It matches at least:";
            //
            // lblToleranceUnit
            //
            lblToleranceUnit.AutoSize = true;
            lblToleranceUnit.Location = new System.Drawing.Point(266, 54);
            lblToleranceUnit.Name = "lblToleranceUnit";
            lblToleranceUnit.Size = new System.Drawing.Size(129, 15);
            lblToleranceUnit.TabIndex = 5;
            lblToleranceUnit.Text = "per R, G, B  (0 - 255)";
            //
            // numTolerance
            //
            numTolerance.Location = new System.Drawing.Point(180, 51);
            numTolerance.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numTolerance.Name = "numTolerance";
            numTolerance.Size = new System.Drawing.Size(80, 23);
            numTolerance.TabIndex = 4;
            numTolerance.Value = new decimal(new int[] { 20, 0, 0, 0 });
            //
            // lblTolerance
            //
            lblTolerance.AutoSize = true;
            lblTolerance.Location = new System.Drawing.Point(10, 54);
            lblTolerance.Name = "lblTolerance";
            lblTolerance.Size = new System.Drawing.Size(164, 15);
            lblTolerance.TabIndex = 3;
            lblTolerance.Text = "Color tolerance of a pixel:";
            //
            // lblIntervalUnit
            //
            lblIntervalUnit.AutoSize = true;
            lblIntervalUnit.Location = new System.Drawing.Point(266, 25);
            lblIntervalUnit.Name = "lblIntervalUnit";
            lblIntervalUnit.Size = new System.Drawing.Size(23, 15);
            lblIntervalUnit.TabIndex = 2;
            lblIntervalUnit.Text = "ms";
            //
            // numInterval
            //
            numInterval.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            numInterval.Location = new System.Drawing.Point(180, 22);
            numInterval.Maximum = new decimal(new int[] { 3600000, 0, 0, 0 });
            numInterval.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numInterval.Name = "numInterval";
            numInterval.Size = new System.Drawing.Size(80, 23);
            numInterval.TabIndex = 1;
            numInterval.Value = new decimal(new int[] { 500, 0, 0, 0 });
            //
            // lblInterval
            //
            lblInterval.AutoSize = true;
            lblInterval.Location = new System.Drawing.Point(10, 25);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new System.Drawing.Size(126, 15);
            lblInterval.TabIndex = 0;
            lblInterval.Text = "Capture the area every:";
            //
            // lblTarget
            //
            lblTarget.AutoSize = true;
            lblTarget.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            lblTarget.Location = new System.Drawing.Point(12, 410);
            lblTarget.Name = "lblTarget";
            lblTarget.Size = new System.Drawing.Size(72, 15);
            lblTarget.TabIndex = 3;
            lblTarget.Text = "Then go to:";
            //
            // cboTarget
            //
            cboTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboTarget.FormattingEnabled = true;
            cboTarget.Location = new System.Drawing.Point(90, 407);
            cboTarget.Name = "cboTarget";
            cboTarget.Size = new System.Drawing.Size(342, 23);
            cboTarget.TabIndex = 4;
            //
            // btnOk
            //
            btnOk.Location = new System.Drawing.Point(246, 444);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(90, 27);
            btnOk.TabIndex = 5;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            //
            // btnCancel
            //
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(342, 444);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(90, 27);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            //
            // ConditionForm
            //
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(444, 483);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(cboTarget);
            Controls.Add(lblTarget);
            Controls.Add(lblValueUnit);
            Controls.Add(numValue);
            Controls.Add(lblValue);
            Controls.Add(grpScreen);
            Controls.Add(cboType);
            Controls.Add(lblType);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConditionForm";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Condition";
            FormClosed += ConditionForm_FormClosed;
            Load += ConditionForm_Load;
            ((System.ComponentModel.ISupportInitialize)numValue).EndInit();
            grpScreen.ResumeLayout(false);
            grpScreen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picSample).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPercent).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTolerance).EndInit();
            ((System.ComponentModel.ISupportInitialize)numInterval).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.NumericUpDown numValue;
        private System.Windows.Forms.Label lblValueUnit;
        private System.Windows.Forms.GroupBox grpScreen;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.Label lblIntervalUnit;
        private System.Windows.Forms.Label lblTolerance;
        private System.Windows.Forms.NumericUpDown numTolerance;
        private System.Windows.Forms.Label lblToleranceUnit;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.NumericUpDown numPercent;
        private System.Windows.Forms.Label lblPercentUnit;
        private System.Windows.Forms.Label lblSample;
        private System.Windows.Forms.PictureBox picSample;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label lblTestResult;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.ComboBox cboTarget;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
