namespace TestCreateBREAD
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.bottomPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.upperPanel = new System.Windows.Forms.TableLayoutPanel();
            this.txtFilterSchema = new System.Windows.Forms.TextBox();
            this.lblFilterSchemaDescription = new System.Windows.Forms.Label();
            this.btnFilter = new System.Windows.Forms.Button();
            this.lblFilterTableDescription = new System.Windows.Forms.Label();
            this.txtFilterTable = new System.Windows.Forms.TextBox();
            this.gvTables = new System.Windows.Forms.DataGridView();
            this.pnlMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ttFilterUsage = new System.Windows.Forms.ToolTip(this.components);
            this.mainPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.upperPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvTables)).BeginInit();
            this.pnlMenu.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Controls.Add(this.bottomPanel, 0, 3);
            this.mainPanel.Controls.Add(this.upperPanel, 0, 1);
            this.mainPanel.Controls.Add(this.gvTables, 0, 2);
            this.mainPanel.Controls.Add(this.pnlMenu, 0, 0);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 4;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.mainPanel.Size = new System.Drawing.Size(822, 468);
            this.mainPanel.TabIndex = 0;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.btnGenerate);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomPanel.Location = new System.Drawing.Point(3, 436);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bottomPanel.Size = new System.Drawing.Size(816, 29);
            this.bottomPanel.TabIndex = 0;
            // 
            // btnGenerate
            // 
            this.btnGenerate.AutoSize = true;
            this.btnGenerate.Enabled = false;
            this.btnGenerate.Location = new System.Drawing.Point(707, 3);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(106, 23);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "Generate BREADs";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // upperPanel
            // 
            this.upperPanel.ColumnCount = 5;
            this.upperPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.upperPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.upperPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.upperPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.upperPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.upperPanel.Controls.Add(this.txtFilterSchema, 0, 0);
            this.upperPanel.Controls.Add(this.lblFilterSchemaDescription, 0, 0);
            this.upperPanel.Controls.Add(this.btnFilter, 4, 0);
            this.upperPanel.Controls.Add(this.lblFilterTableDescription, 2, 0);
            this.upperPanel.Controls.Add(this.txtFilterTable, 3, 0);
            this.upperPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.upperPanel.Location = new System.Drawing.Point(3, 28);
            this.upperPanel.Name = "upperPanel";
            this.upperPanel.RowCount = 1;
            this.upperPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.upperPanel.Size = new System.Drawing.Size(816, 29);
            this.upperPanel.TabIndex = 1;
            this.ttFilterUsage.SetToolTip(this.upperPanel, "Separate each filter using a semicolon (;).\r\nEach filter will be treated in an \"i" +
        "nclusive-like\" fashion (\"%<filter>%\")");
            // 
            // txtFilterSchema
            // 
            this.txtFilterSchema.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilterSchema.Enabled = false;
            this.txtFilterSchema.Location = new System.Drawing.Point(78, 3);
            this.txtFilterSchema.Name = "txtFilterSchema";
            this.txtFilterSchema.Size = new System.Drawing.Size(172, 20);
            this.txtFilterSchema.TabIndex = 4;
            this.txtFilterSchema.Text = "dbo";
            this.ttFilterUsage.SetToolTip(this.txtFilterSchema, "Separate each filter using a semicolon (;).\r\nEach filter will be treated in an \"i" +
        "nclusive-like\" fashion (\"%<filter>%\")");
            // 
            // lblFilterSchemaDescription
            // 
            this.lblFilterSchemaDescription.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFilterSchemaDescription.AutoSize = true;
            this.lblFilterSchemaDescription.Location = new System.Drawing.Point(3, 8);
            this.lblFilterSchemaDescription.Name = "lblFilterSchemaDescription";
            this.lblFilterSchemaDescription.Size = new System.Drawing.Size(69, 13);
            this.lblFilterSchemaDescription.TabIndex = 3;
            this.lblFilterSchemaDescription.Text = "Filter schema";
            this.ttFilterUsage.SetToolTip(this.lblFilterSchemaDescription, "Separate each filter using a semicolon (;).\r\nEach filter will be treated in an \"i" +
        "nclusive-like\" fashion (\"%<filter>%\")");
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(737, 3);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 2;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // lblFilterTableDescription
            // 
            this.lblFilterTableDescription.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFilterTableDescription.AutoSize = true;
            this.lblFilterTableDescription.Location = new System.Drawing.Point(256, 8);
            this.lblFilterTableDescription.Name = "lblFilterTableDescription";
            this.lblFilterTableDescription.Size = new System.Drawing.Size(60, 13);
            this.lblFilterTableDescription.TabIndex = 0;
            this.lblFilterTableDescription.Text = "Filter tables";
            this.ttFilterUsage.SetToolTip(this.lblFilterTableDescription, "Separate each filter using a semicolon (;).\r\nEach filter will be treated in an \"i" +
        "nclusive-like\" fashion (\"%<filter>%\")");
            // 
            // txtFilterTable
            // 
            this.txtFilterTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilterTable.Location = new System.Drawing.Point(322, 3);
            this.txtFilterTable.Name = "txtFilterTable";
            this.txtFilterTable.Size = new System.Drawing.Size(409, 20);
            this.txtFilterTable.TabIndex = 1;
            this.ttFilterUsage.SetToolTip(this.txtFilterTable, "Separate each filter using a semicolon (;).\r\nEach filter will be treated in an \"i" +
        "nclusive-like\" fashion (\"%<filter>%\")");
            // 
            // gvTables
            // 
            this.gvTables.AllowUserToAddRows = false;
            this.gvTables.AllowUserToDeleteRows = false;
            this.gvTables.AllowUserToResizeRows = false;
            this.gvTables.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvTables.Location = new System.Drawing.Point(3, 63);
            this.gvTables.Name = "gvTables";
            this.gvTables.ReadOnly = true;
            this.gvTables.Size = new System.Drawing.Size(816, 367);
            this.gvTables.TabIndex = 3;
            this.gvTables.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gvTables_DataBindingComplete);
            this.gvTables.SelectionChanged += new System.EventHandler(this.gvTables_SelectionChanged);
            // 
            // pnlMenu
            // 
            this.pnlMenu.Controls.Add(this.mainMenuStrip);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(822, 25);
            this.pnlMenu.TabIndex = 4;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(45, 24);
            this.mainMenuStrip.TabIndex = 6;
            this.mainMenuStrip.Text = "File";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.saveToolStripMenuItem.Text = "&Export selection to file";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.openToolStripMenuItem.Text = "&Import selection from file";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xml";
            this.saveFileDialog.Filter = "xml files|*.xml";
            this.saveFileDialog.Title = "Save current selection";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xml";
            this.openFileDialog.Filter = "xml files|*.xml";
            this.openFileDialog.Title = "Open a saved selection";
            // 
            // ttFilterUsage
            // 
            this.ttFilterUsage.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ttFilterUsage.ToolTipTitle = "Filter usage";
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnFilter;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 468);
            this.Controls.Add(this.mainPanel);
            this.Name = "MainForm";
            this.Text = "Custom BREADs generator";
            this.mainPanel.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.upperPanel.ResumeLayout(false);
            this.upperPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvTables)).EndInit();
            this.pnlMenu.ResumeLayout(false);
            this.pnlMenu.PerformLayout();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.FlowLayoutPanel bottomPanel;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TableLayoutPanel upperPanel;
        private System.Windows.Forms.Label lblFilterTableDescription;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.TextBox txtFilterTable;
        private System.Windows.Forms.DataGridView gvTables;
        private System.Windows.Forms.TextBox txtFilterSchema;
        private System.Windows.Forms.Label lblFilterSchemaDescription;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel pnlMenu;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolTip ttFilterUsage;

    }
}

