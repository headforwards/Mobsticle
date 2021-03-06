﻿namespace Mobsticle
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.txtParticipants = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numMinutes = new System.Windows.Forms.NumericUpDown();
            this.btnClose = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mniSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mniStart = new System.Windows.Forms.ToolStripMenuItem();
            this.mniPause = new System.Windows.Forms.ToolStripMenuItem();
            this.mniRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.mniExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.cboNotification = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numMinutes)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Participants";
            // 
            // txtParticipants
            // 
            this.txtParticipants.AcceptsReturn = true;
            this.txtParticipants.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParticipants.Location = new System.Drawing.Point(16, 30);
            this.txtParticipants.Multiline = true;
            this.txtParticipants.Name = "txtParticipants";
            this.txtParticipants.Size = new System.Drawing.Size(211, 142);
            this.txtParticipants.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Minutes";
            // 
            // numMinutes
            // 
            this.numMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numMinutes.Location = new System.Drawing.Point(16, 204);
            this.numMinutes.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinutes.Name = "numMinutes";
            this.numMinutes.Size = new System.Drawing.Size(78, 20);
            this.numMinutes.TabIndex = 3;
            this.numMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(152, 289);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Text = "Mobsticle";
            this.notifyIcon.Visible = true;
            this.notifyIcon.BalloonTipClicked += new System.EventHandler(this.NotifyIcon_BalloonTipClicked);
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_Click);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniSettings,
            this.mniStart,
            this.mniPause,
            this.mniRotate,
            this.mniExit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(117, 114);
            // 
            // mniSettings
            // 
            this.mniSettings.Name = "mniSettings";
            this.mniSettings.Size = new System.Drawing.Size(116, 22);
            this.mniSettings.Text = "Settings";
            this.mniSettings.Click += new System.EventHandler(this.mniSettings_Click);
            // 
            // mniStart
            // 
            this.mniStart.Name = "mniStart";
            this.mniStart.Size = new System.Drawing.Size(116, 22);
            this.mniStart.Text = "Start";
            this.mniStart.Click += new System.EventHandler(this.MniStart_Click);
            // 
            // mniPause
            // 
            this.mniPause.Name = "mniPause";
            this.mniPause.Size = new System.Drawing.Size(116, 22);
            this.mniPause.Text = "Pause";
            this.mniPause.Click += new System.EventHandler(this.mniPause_Click);
            // 
            // mniRotate
            // 
            this.mniRotate.Name = "mniRotate";
            this.mniRotate.Size = new System.Drawing.Size(116, 22);
            this.mniRotate.Text = "Rotate";
            this.mniRotate.Click += new System.EventHandler(this.MniRotate_Click);
            // 
            // mniExit
            // 
            this.mniExit.Name = "mniExit";
            this.mniExit.Size = new System.Drawing.Size(116, 22);
            this.mniExit.Text = "Exit";
            this.mniExit.Click += new System.EventHandler(this.mniExit_Click);
            // 
            // timer
            // 
            this.timer.Interval = 500;
            // 
            // cboNotification
            // 
            this.cboNotification.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboNotification.DisplayMember = "Key";
            this.cboNotification.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNotification.FormattingEnabled = true;
            this.cboNotification.Location = new System.Drawing.Point(16, 252);
            this.cboNotification.Name = "cboNotification";
            this.cboNotification.Size = new System.Drawing.Size(211, 21);
            this.cboNotification.TabIndex = 4;
            this.cboNotification.ValueMember = "Value";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 236);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Notification";
            // 
            // Main
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(239, 326);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboNotification);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.numMinutes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtParticipants);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mobsticle";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numMinutes)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtParticipants;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMinutes;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mniSettings;
        private System.Windows.Forms.ToolStripMenuItem mniPause;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripMenuItem mniExit;
        private System.Windows.Forms.ComboBox cboNotification;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem mniStart;
        private System.Windows.Forms.ToolStripMenuItem mniRotate;
    }
}

