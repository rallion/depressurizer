namespace Depressurizer {
    partial class OptionsDlg {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.grpSteamDir = new System.Windows.Forms.GroupBox();
            this.cmdSteamPathBrowse = new System.Windows.Forms.Button();
            this.txtSteamPath = new System.Windows.Forms.TextBox();
            this.grpStartup = new System.Windows.Forms.GroupBox();
            this.cmdDefaultProfileBrowse = new System.Windows.Forms.Button();
            this.txtDefaultProfile = new System.Windows.Forms.TextBox();
            this.chkPLoadUpdateGameList = new System.Windows.Forms.CheckBox();
            this.chkPLoadSteamCats = new System.Windows.Forms.CheckBox();
            this.chkAutoSaveToSteam = new System.Windows.Forms.CheckBox();
            this.chkRemoveUnwantedEntries = new System.Windows.Forms.CheckBox();
            this.cmdAccept = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.grpLoading = new System.Windows.Forms.GroupBox();
            this.grpSaving = new System.Windows.Forms.GroupBox();
            this.chkLoadProfileOnStart = new System.Windows.Forms.CheckBox();
            this.grpSteamDir.SuspendLayout();
            this.grpStartup.SuspendLayout();
            this.grpLoading.SuspendLayout();
            this.grpSaving.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSteamDir
            // 
            this.grpSteamDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSteamDir.Controls.Add(this.cmdSteamPathBrowse);
            this.grpSteamDir.Controls.Add(this.txtSteamPath);
            this.grpSteamDir.Location = new System.Drawing.Point(12, 12);
            this.grpSteamDir.Name = "grpSteamDir";
            this.grpSteamDir.Size = new System.Drawing.Size(473, 54);
            this.grpSteamDir.TabIndex = 0;
            this.grpSteamDir.TabStop = false;
            this.grpSteamDir.Text = "Steam Directory";
            // 
            // cmdSteamPathBrowse
            // 
            this.cmdSteamPathBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSteamPathBrowse.Location = new System.Drawing.Point(389, 20);
            this.cmdSteamPathBrowse.Name = "cmdSteamPathBrowse";
            this.cmdSteamPathBrowse.Size = new System.Drawing.Size(75, 23);
            this.cmdSteamPathBrowse.TabIndex = 1;
            this.cmdSteamPathBrowse.Text = "Browse...";
            this.cmdSteamPathBrowse.UseVisualStyleBackColor = true;
            this.cmdSteamPathBrowse.Click += new System.EventHandler(this.cmdSteamPathBrowse_Click);
            // 
            // txtSteamPath
            // 
            this.txtSteamPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSteamPath.Location = new System.Drawing.Point(11, 22);
            this.txtSteamPath.Name = "txtSteamPath";
            this.txtSteamPath.Size = new System.Drawing.Size(372, 20);
            this.txtSteamPath.TabIndex = 0;
            // 
            // grpStartup
            // 
            this.grpStartup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpStartup.Controls.Add(this.chkLoadProfileOnStart);
            this.grpStartup.Controls.Add(this.cmdDefaultProfileBrowse);
            this.grpStartup.Controls.Add(this.txtDefaultProfile);
            this.grpStartup.Location = new System.Drawing.Point(12, 72);
            this.grpStartup.Name = "grpStartup";
            this.grpStartup.Size = new System.Drawing.Size(473, 50);
            this.grpStartup.TabIndex = 1;
            this.grpStartup.TabStop = false;
            this.grpStartup.Text = "On Startup";
            // 
            // cmdDefaultProfileBrowse
            // 
            this.cmdDefaultProfileBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDefaultProfileBrowse.Location = new System.Drawing.Point(389, 16);
            this.cmdDefaultProfileBrowse.Name = "cmdDefaultProfileBrowse";
            this.cmdDefaultProfileBrowse.Size = new System.Drawing.Size(75, 23);
            this.cmdDefaultProfileBrowse.TabIndex = 3;
            this.cmdDefaultProfileBrowse.Text = "Browse...";
            this.cmdDefaultProfileBrowse.UseVisualStyleBackColor = true;
            this.cmdDefaultProfileBrowse.Click += new System.EventHandler(this.cmdDefaultProfileBrowse_Click);
            // 
            // txtDefaultProfile
            // 
            this.txtDefaultProfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDefaultProfile.Location = new System.Drawing.Point(101, 18);
            this.txtDefaultProfile.Name = "txtDefaultProfile";
            this.txtDefaultProfile.Size = new System.Drawing.Size(282, 20);
            this.txtDefaultProfile.TabIndex = 2;
            // 
            // chkPLoadUpdateGameList
            // 
            this.chkPLoadUpdateGameList.AutoSize = true;
            this.chkPLoadUpdateGameList.Location = new System.Drawing.Point(6, 19);
            this.chkPLoadUpdateGameList.Name = "chkPLoadUpdateGameList";
            this.chkPLoadUpdateGameList.Size = new System.Drawing.Size(247, 17);
            this.chkPLoadUpdateGameList.TabIndex = 2;
            this.chkPLoadUpdateGameList.Text = "Automatically download gamelist on profile load";
            this.chkPLoadUpdateGameList.UseVisualStyleBackColor = true;
            // 
            // chkPLoadSteamCats
            // 
            this.chkPLoadSteamCats.AutoSize = true;
            this.chkPLoadSteamCats.Location = new System.Drawing.Point(6, 42);
            this.chkPLoadSteamCats.Name = "chkPLoadSteamCats";
            this.chkPLoadSteamCats.Size = new System.Drawing.Size(288, 17);
            this.chkPLoadSteamCats.TabIndex = 3;
            this.chkPLoadSteamCats.Text = "Automatically load categories from Steam on profile load";
            this.chkPLoadSteamCats.UseVisualStyleBackColor = true;
            // 
            // chkAutoSaveToSteam
            // 
            this.chkAutoSaveToSteam.AutoSize = true;
            this.chkAutoSaveToSteam.Location = new System.Drawing.Point(6, 19);
            this.chkAutoSaveToSteam.Name = "chkAutoSaveToSteam";
            this.chkAutoSaveToSteam.Size = new System.Drawing.Size(277, 17);
            this.chkAutoSaveToSteam.TabIndex = 4;
            this.chkAutoSaveToSteam.Text = "Automatically save to Steam when saving profile data";
            this.chkAutoSaveToSteam.UseVisualStyleBackColor = true;
            // 
            // chkRemoveUnwantedEntries
            // 
            this.chkRemoveUnwantedEntries.AutoSize = true;
            this.chkRemoveUnwantedEntries.Location = new System.Drawing.Point(6, 42);
            this.chkRemoveUnwantedEntries.Name = "chkRemoveUnwantedEntries";
            this.chkRemoveUnwantedEntries.Size = new System.Drawing.Size(345, 17);
            this.chkRemoveUnwantedEntries.TabIndex = 5;
            this.chkRemoveUnwantedEntries.Text = "Remove entries for deleted or unknown games in Steam settings file";
            this.chkRemoveUnwantedEntries.UseVisualStyleBackColor = true;
            // 
            // cmdAccept
            // 
            this.cmdAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAccept.Location = new System.Drawing.Point(410, 279);
            this.cmdAccept.Name = "cmdAccept";
            this.cmdAccept.Size = new System.Drawing.Size(75, 23);
            this.cmdAccept.TabIndex = 6;
            this.cmdAccept.Text = "OK";
            this.cmdAccept.UseVisualStyleBackColor = true;
            this.cmdAccept.Click += new System.EventHandler(this.cmdAccept_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.Location = new System.Drawing.Point(329, 279);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // grpLoading
            // 
            this.grpLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLoading.Controls.Add(this.chkPLoadUpdateGameList);
            this.grpLoading.Controls.Add(this.chkPLoadSteamCats);
            this.grpLoading.Location = new System.Drawing.Point(12, 128);
            this.grpLoading.Name = "grpLoading";
            this.grpLoading.Size = new System.Drawing.Size(473, 67);
            this.grpLoading.TabIndex = 8;
            this.grpLoading.TabStop = false;
            this.grpLoading.Text = "Profile Loading";
            // 
            // grpSaving
            // 
            this.grpSaving.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSaving.Controls.Add(this.chkRemoveUnwantedEntries);
            this.grpSaving.Controls.Add(this.chkAutoSaveToSteam);
            this.grpSaving.Location = new System.Drawing.Point(12, 201);
            this.grpSaving.Name = "grpSaving";
            this.grpSaving.Size = new System.Drawing.Size(473, 70);
            this.grpSaving.TabIndex = 9;
            this.grpSaving.TabStop = false;
            this.grpSaving.Text = "Saving";
            // 
            // chkLoadProfileOnStart
            // 
            this.chkLoadProfileOnStart.AutoSize = true;
            this.chkLoadProfileOnStart.Location = new System.Drawing.Point(11, 20);
            this.chkLoadProfileOnStart.Name = "chkLoadProfileOnStart";
            this.chkLoadProfileOnStart.Size = new System.Drawing.Size(84, 17);
            this.chkLoadProfileOnStart.TabIndex = 4;
            this.chkLoadProfileOnStart.Text = "Load profile:";
            this.chkLoadProfileOnStart.UseVisualStyleBackColor = true;
            // 
            // OptionsDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 312);
            this.Controls.Add(this.grpSaving);
            this.Controls.Add(this.grpLoading);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdAccept);
            this.Controls.Add(this.grpStartup);
            this.Controls.Add(this.grpSteamDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OptionsDlg";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.grpSteamDir.ResumeLayout(false);
            this.grpSteamDir.PerformLayout();
            this.grpStartup.ResumeLayout(false);
            this.grpStartup.PerformLayout();
            this.grpLoading.ResumeLayout(false);
            this.grpLoading.PerformLayout();
            this.grpSaving.ResumeLayout(false);
            this.grpSaving.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSteamDir;
        private System.Windows.Forms.Button cmdSteamPathBrowse;
        private System.Windows.Forms.TextBox txtSteamPath;
        private System.Windows.Forms.GroupBox grpStartup;
        private System.Windows.Forms.Button cmdDefaultProfileBrowse;
        private System.Windows.Forms.TextBox txtDefaultProfile;
        private System.Windows.Forms.CheckBox chkPLoadUpdateGameList;
        private System.Windows.Forms.CheckBox chkPLoadSteamCats;
        private System.Windows.Forms.CheckBox chkAutoSaveToSteam;
        private System.Windows.Forms.CheckBox chkRemoveUnwantedEntries;
        private System.Windows.Forms.Button cmdAccept;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.GroupBox grpLoading;
        private System.Windows.Forms.GroupBox grpSaving;
        private System.Windows.Forms.CheckBox chkLoadProfileOnStart;
    }
}