namespace ProShare
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
            this.panel = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.menuGroupBox = new System.Windows.Forms.GroupBox();
            this.operationsButton = new System.Windows.Forms.Button();
            this.requestsButton = new System.Windows.Forms.Button();
            this.stackPanel = new ProShare.StackPanel();
            this.Operations = new System.Windows.Forms.TabPage();
            this.operationsTabControl = new System.Windows.Forms.TabControl();
            this.generateTabPage = new System.Windows.Forms.TabPage();
            this.reconstructTabPage = new System.Windows.Forms.TabPage();
            this.udpateTabPage = new System.Windows.Forms.TabPage();
            this.Requests = new System.Windows.Forms.TabPage();
            this.Profile = new System.Windows.Forms.TabPage();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.menuGroupBox.SuspendLayout();
            this.stackPanel.SuspendLayout();
            this.Operations.SuspendLayout();
            this.operationsTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.SystemColors.Control;
            this.panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel.Controls.Add(this.splitContainer);
            this.panel.Location = new System.Drawing.Point(12, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(760, 537);
            this.panel.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.menuGroupBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.stackPanel);
            this.splitContainer.Size = new System.Drawing.Size(756, 533);
            this.splitContainer.SplitterDistance = 207;
            this.splitContainer.TabIndex = 0;
            // 
            // menuGroupBox
            // 
            this.menuGroupBox.Controls.Add(this.requestsButton);
            this.menuGroupBox.Controls.Add(this.operationsButton);
            this.menuGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuGroupBox.Location = new System.Drawing.Point(0, 0);
            this.menuGroupBox.Name = "menuGroupBox";
            this.menuGroupBox.Size = new System.Drawing.Size(207, 533);
            this.menuGroupBox.TabIndex = 0;
            this.menuGroupBox.TabStop = false;
            this.menuGroupBox.Text = "Hello, [USERNAME]!";
            // 
            // operationsButton
            // 
            this.operationsButton.Location = new System.Drawing.Point(10, 32);
            this.operationsButton.Name = "operationsButton";
            this.operationsButton.Size = new System.Drawing.Size(167, 46);
            this.operationsButton.TabIndex = 0;
            this.operationsButton.Text = "Operations";
            this.operationsButton.UseVisualStyleBackColor = true;
            this.operationsButton.Click += new System.EventHandler(this.operationsButton_Click);
            // 
            // requestsButton
            // 
            this.requestsButton.Location = new System.Drawing.Point(10, 98);
            this.requestsButton.Name = "requestsButton";
            this.requestsButton.Size = new System.Drawing.Size(167, 46);
            this.requestsButton.TabIndex = 1;
            this.requestsButton.Text = "Requests";
            this.requestsButton.UseVisualStyleBackColor = true;
            this.requestsButton.Click += new System.EventHandler(this.requestsButton_Click);
            // 
            // stackPanel
            // 
            this.stackPanel.Controls.Add(this.Operations);
            this.stackPanel.Controls.Add(this.Requests);
            this.stackPanel.Controls.Add(this.Profile);
            this.stackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackPanel.Location = new System.Drawing.Point(0, 0);
            this.stackPanel.Name = "stackPanel";
            this.stackPanel.SelectedIndex = 0;
            this.stackPanel.Size = new System.Drawing.Size(545, 533);
            this.stackPanel.TabIndex = 0;
            // 
            // Operations
            // 
            this.Operations.BackColor = System.Drawing.SystemColors.Control;
            this.Operations.Controls.Add(this.operationsTabControl);
            this.Operations.Location = new System.Drawing.Point(4, 22);
            this.Operations.Name = "Operations";
            this.Operations.Padding = new System.Windows.Forms.Padding(3);
            this.Operations.Size = new System.Drawing.Size(537, 507);
            this.Operations.TabIndex = 0;
            this.Operations.Text = "Operations";
            // 
            // operationsTabControl
            // 
            this.operationsTabControl.Controls.Add(this.generateTabPage);
            this.operationsTabControl.Controls.Add(this.reconstructTabPage);
            this.operationsTabControl.Controls.Add(this.udpateTabPage);
            this.operationsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationsTabControl.Location = new System.Drawing.Point(3, 3);
            this.operationsTabControl.Name = "operationsTabControl";
            this.operationsTabControl.SelectedIndex = 0;
            this.operationsTabControl.Size = new System.Drawing.Size(531, 501);
            this.operationsTabControl.TabIndex = 0;
            // 
            // generateTabPage
            // 
            this.generateTabPage.Location = new System.Drawing.Point(4, 22);
            this.generateTabPage.Name = "generateTabPage";
            this.generateTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generateTabPage.Size = new System.Drawing.Size(523, 475);
            this.generateTabPage.TabIndex = 0;
            this.generateTabPage.Text = "Generate Shares";
            this.generateTabPage.UseVisualStyleBackColor = true;
            // 
            // reconstructTabPage
            // 
            this.reconstructTabPage.Location = new System.Drawing.Point(4, 22);
            this.reconstructTabPage.Name = "reconstructTabPage";
            this.reconstructTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.reconstructTabPage.Size = new System.Drawing.Size(523, 475);
            this.reconstructTabPage.TabIndex = 1;
            this.reconstructTabPage.Text = "Reconstruct Secret";
            this.reconstructTabPage.UseVisualStyleBackColor = true;
            // 
            // udpateTabPage
            // 
            this.udpateTabPage.Location = new System.Drawing.Point(4, 22);
            this.udpateTabPage.Name = "udpateTabPage";
            this.udpateTabPage.Size = new System.Drawing.Size(523, 475);
            this.udpateTabPage.TabIndex = 2;
            this.udpateTabPage.Text = "Update Shares";
            this.udpateTabPage.UseVisualStyleBackColor = true;
            // 
            // Requests
            // 
            this.Requests.BackColor = System.Drawing.SystemColors.Control;
            this.Requests.Location = new System.Drawing.Point(4, 22);
            this.Requests.Name = "Requests";
            this.Requests.Size = new System.Drawing.Size(537, 507);
            this.Requests.TabIndex = 2;
            this.Requests.Text = "Requests";
            // 
            // Profile
            // 
            this.Profile.BackColor = System.Drawing.SystemColors.Control;
            this.Profile.Location = new System.Drawing.Point(4, 22);
            this.Profile.Name = "Profile";
            this.Profile.Padding = new System.Windows.Forms.Padding(3);
            this.Profile.Size = new System.Drawing.Size(537, 507);
            this.Profile.TabIndex = 1;
            this.Profile.Text = "Profile";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panel);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.menuGroupBox.ResumeLayout(false);
            this.stackPanel.ResumeLayout(false);
            this.Operations.ResumeLayout(false);
            this.operationsTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox menuGroupBox;
        private System.Windows.Forms.TabControl operationsTabControl;
        private System.Windows.Forms.TabPage generateTabPage;
        private System.Windows.Forms.TabPage reconstructTabPage;
        private System.Windows.Forms.TabPage udpateTabPage;
        private System.Windows.Forms.Button operationsButton;
        private StackPanel stackPanel;
        private System.Windows.Forms.TabPage Operations;
        private System.Windows.Forms.TabPage Profile;
        private System.Windows.Forms.TabPage Requests;
        private System.Windows.Forms.Button requestsButton;
    }
}