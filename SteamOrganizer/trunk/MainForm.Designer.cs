namespace SteamOrganizer {
    partial class FormMain {
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsGetList = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.grpCategories = new System.Windows.Forms.GroupBox();
            this.lstCategories = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmdCatAdd = new System.Windows.Forms.Button();
            this.cmdCatDelete = new System.Windows.Forms.Button();
            this.cmdCatRename = new System.Windows.Forms.Button();
            this.grpGames = new System.Windows.Forms.GroupBox();
            this.lstGames = new System.Windows.Forms.ListView();
            this.colGameID = ( (System.Windows.Forms.ColumnHeader)( new System.Windows.Forms.ColumnHeader() ) );
            this.colTitle = ( (System.Windows.Forms.ColumnHeader)( new System.Windows.Forms.ColumnHeader() ) );
            this.colCategory = ( (System.Windows.Forms.ColumnHeader)( new System.Windows.Forms.ColumnHeader() ) );
            this.colFavorite = ( (System.Windows.Forms.ColumnHeader)( new System.Windows.Forms.ColumnHeader() ) );
            this.menuStrip.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.splitContainer ) ).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.grpCategories.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpGames.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuTools} );
            this.menuStrip.Location = new System.Drawing.Point( 0, 0 );
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size( 938, 24 );
            this.menuStrip.TabIndex = 4;
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.menuFileSave,
            this.menuFileLoad,
            this.menuFileExit} );
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size( 37, 20 );
            this.menuFile.Text = "File";
            // 
            // menuFileSave
            // 
            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSave.Size = new System.Drawing.Size( 100, 22 );
            this.menuFileSave.Text = "Save";
            // 
            // menuFileLoad
            // 
            this.menuFileLoad.Name = "menuFileLoad";
            this.menuFileLoad.Size = new System.Drawing.Size( 100, 22 );
            this.menuFileLoad.Text = "Load";
            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Size = new System.Drawing.Size( 100, 22 );
            this.menuFileExit.Text = "Exit";
            // 
            // menuTools
            // 
            this.menuTools.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.menuToolsOptions,
            this.menuToolsGetList} );
            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new System.Drawing.Size( 48, 20 );
            this.menuTools.Text = "Tools";
            // 
            // menuToolsOptions
            // 
            this.menuToolsOptions.Name = "menuToolsOptions";
            this.menuToolsOptions.Size = new System.Drawing.Size( 147, 22 );
            this.menuToolsOptions.Text = "Options";
            // 
            // menuToolsGetList
            // 
            this.menuToolsGetList.Name = "menuToolsGetList";
            this.menuToolsGetList.Size = new System.Drawing.Size( 147, 22 );
            this.menuToolsGetList.Text = "Get Game List";
            this.menuToolsGetList.Click += new System.EventHandler( this.menuToolsGetList_Click );
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point( 0, 24 );
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add( this.grpCategories );
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add( this.grpGames );
            this.splitContainer.Size = new System.Drawing.Size( 938, 432 );
            this.splitContainer.SplitterDistance = 312;
            this.splitContainer.TabIndex = 5;
            // 
            // grpCategories
            // 
            this.grpCategories.Controls.Add( this.lstCategories );
            this.grpCategories.Controls.Add( this.tableLayoutPanel1 );
            this.grpCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpCategories.Location = new System.Drawing.Point( 0, 0 );
            this.grpCategories.Name = "grpCategories";
            this.grpCategories.Size = new System.Drawing.Size( 312, 432 );
            this.grpCategories.TabIndex = 0;
            this.grpCategories.TabStop = false;
            this.grpCategories.Text = "Categories";
            // 
            // lstCategories
            // 
            this.lstCategories.AllowDrop = true;
            this.lstCategories.FullRowSelect = true;
            this.lstCategories.HideSelection = false;
            this.lstCategories.LabelEdit = true;
            this.lstCategories.Location = new System.Drawing.Point( 3, 16 );
            this.lstCategories.MultiSelect = false;
            this.lstCategories.Name = "lstCategories";
            this.lstCategories.Size = new System.Drawing.Size( 306, 377 );
            this.lstCategories.TabIndex = 2;
            this.lstCategories.UseCompatibleStateImageBehavior = false;
            this.lstCategories.View = System.Windows.Forms.View.List;
            this.lstCategories.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler( this.lstCategories_BeforeLabelEdit );
            this.lstCategories.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler( this.lstCategories_ItemSelectionChanged );
            this.lstCategories.DragDrop += new System.Windows.Forms.DragEventHandler( this.lstCategories_DragDrop );
            this.lstCategories.DragEnter += new System.Windows.Forms.DragEventHandler( this.lstCategories_DragEnter );
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 33.33333F ) );
            this.tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 33.33333F ) );
            this.tableLayoutPanel1.ColumnStyles.Add( new System.Windows.Forms.ColumnStyle( System.Windows.Forms.SizeType.Percent, 33.33333F ) );
            this.tableLayoutPanel1.Controls.Add( this.cmdCatAdd, 0, 0 );
            this.tableLayoutPanel1.Controls.Add( this.cmdCatDelete, 2, 0 );
            this.tableLayoutPanel1.Controls.Add( this.cmdCatRename, 1, 0 );
            this.tableLayoutPanel1.Location = new System.Drawing.Point( 3, 396 );
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add( new System.Windows.Forms.RowStyle( System.Windows.Forms.SizeType.Percent, 100F ) );
            this.tableLayoutPanel1.Size = new System.Drawing.Size( 309, 30 );
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // cmdCatAdd
            // 
            this.cmdCatAdd.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cmdCatAdd.Location = new System.Drawing.Point( 3, 3 );
            this.cmdCatAdd.Name = "cmdCatAdd";
            this.cmdCatAdd.Size = new System.Drawing.Size( 97, 24 );
            this.cmdCatAdd.TabIndex = 0;
            this.cmdCatAdd.Text = "Add";
            this.cmdCatAdd.UseVisualStyleBackColor = true;
            this.cmdCatAdd.Click += new System.EventHandler( this.cmdCatAdd_Click );
            // 
            // cmdCatDelete
            // 
            this.cmdCatDelete.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cmdCatDelete.Location = new System.Drawing.Point( 209, 3 );
            this.cmdCatDelete.Name = "cmdCatDelete";
            this.cmdCatDelete.Size = new System.Drawing.Size( 97, 24 );
            this.cmdCatDelete.TabIndex = 1;
            this.cmdCatDelete.Text = "Delete";
            this.cmdCatDelete.UseVisualStyleBackColor = true;
            // 
            // cmdCatRename
            // 
            this.cmdCatRename.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cmdCatRename.Location = new System.Drawing.Point( 106, 3 );
            this.cmdCatRename.Name = "cmdCatRename";
            this.cmdCatRename.Size = new System.Drawing.Size( 97, 24 );
            this.cmdCatRename.TabIndex = 2;
            this.cmdCatRename.Text = "Rename";
            this.cmdCatRename.UseVisualStyleBackColor = true;
            // 
            // grpGames
            // 
            this.grpGames.Controls.Add( this.lstGames );
            this.grpGames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpGames.Location = new System.Drawing.Point( 0, 0 );
            this.grpGames.Name = "grpGames";
            this.grpGames.Size = new System.Drawing.Size( 622, 432 );
            this.grpGames.TabIndex = 0;
            this.grpGames.TabStop = false;
            this.grpGames.Text = "Games";
            // 
            // lstGames
            // 
            this.lstGames.AllowDrop = true;
            this.lstGames.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.lstGames.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.colGameID,
            this.colTitle,
            this.colCategory,
            this.colFavorite} );
            this.lstGames.FullRowSelect = true;
            this.lstGames.GridLines = true;
            this.lstGames.Location = new System.Drawing.Point( 3, 16 );
            this.lstGames.Name = "lstGames";
            this.lstGames.Size = new System.Drawing.Size( 616, 368 );
            this.lstGames.TabIndex = 0;
            this.lstGames.UseCompatibleStateImageBehavior = false;
            this.lstGames.View = System.Windows.Forms.View.Details;
            this.lstGames.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler( this.lstGames_ColumnClick );
            this.lstGames.ItemDrag += new System.Windows.Forms.ItemDragEventHandler( this.lstGames_ItemDrag );
            // 
            // colGameID
            // 
            this.colGameID.Text = "Game ID";
            // 
            // colTitle
            // 
            this.colTitle.Text = "Title";
            this.colTitle.Width = 353;
            // 
            // colCategory
            // 
            this.colCategory.Text = "Category";
            this.colCategory.Width = 124;
            // 
            // colFavorite
            // 
            this.colFavorite.Text = "Favorite";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 938, 456 );
            this.Controls.Add( this.splitContainer );
            this.Controls.Add( this.menuStrip );
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FormMain";
            this.Text = "Depressurizer";
            this.menuStrip.ResumeLayout( false );
            this.menuStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout( false );
            this.splitContainer.Panel2.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize)( this.splitContainer ) ).EndInit();
            this.splitContainer.ResumeLayout( false );
            this.grpCategories.ResumeLayout( false );
            this.tableLayoutPanel1.ResumeLayout( false );
            this.grpGames.ResumeLayout( false );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuFileLoad;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuToolsOptions;
        private System.Windows.Forms.ToolStripMenuItem menuToolsGetList;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox grpCategories;
        private System.Windows.Forms.GroupBox grpGames;
        private System.Windows.Forms.ListView lstGames;
        private System.Windows.Forms.ColumnHeader colTitle;
        private System.Windows.Forms.ColumnHeader colCategory;
        private System.Windows.Forms.ColumnHeader colGameID;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button cmdCatAdd;
        private System.Windows.Forms.Button cmdCatDelete;
        private System.Windows.Forms.Button cmdCatRename;
        private System.Windows.Forms.ListView lstCategories;
        private System.Windows.Forms.ColumnHeader colFavorite;
    }
}

