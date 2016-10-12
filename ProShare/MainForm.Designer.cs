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
            this.requestsButton = new System.Windows.Forms.Button();
            this.operationsButton = new System.Windows.Forms.Button();
            this.genOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.stackPanel = new ProShare.StackPanel();
            this.Operations = new System.Windows.Forms.TabPage();
            this.operationsTabControl = new System.Windows.Forms.TabControl();
            this.generateTabPage = new System.Windows.Forms.TabPage();
            this.genStatusPanel = new System.Windows.Forms.Panel();
            this.genDontCloseLabel = new System.Windows.Forms.Label();
            this.genStatusLabel = new System.Windows.Forms.Label();
            this.genShareButton = new System.Windows.Forms.Button();
            this.genPlayersGroupBox = new System.Windows.Forms.GroupBox();
            this.genRemoveButton = new System.Windows.Forms.Button();
            this.genAddButton = new System.Windows.Forms.Button();
            this.genPlayerTextBox = new System.Windows.Forms.TextBox();
            this.genPlayersListBox = new System.Windows.Forms.ListBox();
            this.genSchemeGroupBox = new System.Windows.Forms.GroupBox();
            this.genNNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.genKNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.genNLabel = new System.Windows.Forms.Label();
            this.genKLabel = new System.Windows.Forms.Label();
            this.genSecretGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.genTextBox = new System.Windows.Forms.TextBox();
            this.genFileTextBox = new System.Windows.Forms.TextBox();
            this.genBrowseButton = new System.Windows.Forms.Button();
            this.genNameGroupBox = new System.Windows.Forms.GroupBox();
            this.genNameTextBox = new System.Windows.Forms.TextBox();
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
            this.generateTabPage.SuspendLayout();
            this.genStatusPanel.SuspendLayout();
            this.genPlayersGroupBox.SuspendLayout();
            this.genSchemeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.genNNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.genKNumericUpDown)).BeginInit();
            this.genSecretGroupBox.SuspendLayout();
            this.genNameGroupBox.SuspendLayout();
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
            // requestsButton
            // 
            this.requestsButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.requestsButton.Location = new System.Drawing.Point(3, 62);
            this.requestsButton.Name = "requestsButton";
            this.requestsButton.Size = new System.Drawing.Size(201, 46);
            this.requestsButton.TabIndex = 1;
            this.requestsButton.Text = "Requests";
            this.requestsButton.UseVisualStyleBackColor = true;
            this.requestsButton.Click += new System.EventHandler(this.requestsButton_Click);
            // 
            // operationsButton
            // 
            this.operationsButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.operationsButton.Location = new System.Drawing.Point(3, 16);
            this.operationsButton.Name = "operationsButton";
            this.operationsButton.Size = new System.Drawing.Size(201, 46);
            this.operationsButton.TabIndex = 0;
            this.operationsButton.Text = "Operations";
            this.operationsButton.UseVisualStyleBackColor = true;
            this.operationsButton.Click += new System.EventHandler(this.operationsButton_Click);
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
            this.generateTabPage.Controls.Add(this.genStatusPanel);
            this.generateTabPage.Controls.Add(this.genPlayersGroupBox);
            this.generateTabPage.Controls.Add(this.genSchemeGroupBox);
            this.generateTabPage.Controls.Add(this.genSecretGroupBox);
            this.generateTabPage.Controls.Add(this.genNameGroupBox);
            this.generateTabPage.Location = new System.Drawing.Point(4, 22);
            this.generateTabPage.Name = "generateTabPage";
            this.generateTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generateTabPage.Size = new System.Drawing.Size(523, 475);
            this.generateTabPage.TabIndex = 0;
            this.generateTabPage.Text = "Generate Shares";
            this.generateTabPage.UseVisualStyleBackColor = true;
            // 
            // genStatusPanel
            // 
            this.genStatusPanel.Controls.Add(this.genDontCloseLabel);
            this.genStatusPanel.Controls.Add(this.genStatusLabel);
            this.genStatusPanel.Controls.Add(this.genShareButton);
            this.genStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.genStatusPanel.Location = new System.Drawing.Point(3, 407);
            this.genStatusPanel.Name = "genStatusPanel";
            this.genStatusPanel.Size = new System.Drawing.Size(517, 62);
            this.genStatusPanel.TabIndex = 5;
            // 
            // genDontCloseLabel
            // 
            this.genDontCloseLabel.AutoSize = true;
            this.genDontCloseLabel.Location = new System.Drawing.Point(3, 20);
            this.genDontCloseLabel.Name = "genDontCloseLabel";
            this.genDontCloseLabel.Size = new System.Drawing.Size(152, 13);
            this.genDontCloseLabel.TabIndex = 7;
            this.genDontCloseLabel.Text = "Please don\'t close the program";
            // 
            // genStatusLabel
            // 
            this.genStatusLabel.AutoSize = true;
            this.genStatusLabel.Location = new System.Drawing.Point(3, 6);
            this.genStatusLabel.Name = "genStatusLabel";
            this.genStatusLabel.Size = new System.Drawing.Size(214, 13);
            this.genStatusLabel.TabIndex = 6;
            this.genStatusLabel.Text = "Sending share request to [PLAYER] (1/n) ...";
            // 
            // genShareButton
            // 
            this.genShareButton.Location = new System.Drawing.Point(375, 7);
            this.genShareButton.Name = "genShareButton";
            this.genShareButton.Size = new System.Drawing.Size(135, 39);
            this.genShareButton.TabIndex = 5;
            this.genShareButton.Text = "Send Share Requests";
            this.genShareButton.UseVisualStyleBackColor = true;
            this.genShareButton.Click += new System.EventHandler(this.genShareButton_Click);
            // 
            // genPlayersGroupBox
            // 
            this.genPlayersGroupBox.Controls.Add(this.genRemoveButton);
            this.genPlayersGroupBox.Controls.Add(this.genAddButton);
            this.genPlayersGroupBox.Controls.Add(this.genPlayerTextBox);
            this.genPlayersGroupBox.Controls.Add(this.genPlayersListBox);
            this.genPlayersGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.genPlayersGroupBox.Location = new System.Drawing.Point(3, 253);
            this.genPlayersGroupBox.Name = "genPlayersGroupBox";
            this.genPlayersGroupBox.Size = new System.Drawing.Size(517, 154);
            this.genPlayersGroupBox.TabIndex = 4;
            this.genPlayersGroupBox.TabStop = false;
            this.genPlayersGroupBox.Text = "Add players";
            // 
            // genRemoveButton
            // 
            this.genRemoveButton.Location = new System.Drawing.Point(375, 57);
            this.genRemoveButton.Name = "genRemoveButton";
            this.genRemoveButton.Size = new System.Drawing.Size(70, 22);
            this.genRemoveButton.TabIndex = 9;
            this.genRemoveButton.Text = "Remove";
            this.genRemoveButton.UseVisualStyleBackColor = true;
            this.genRemoveButton.Click += new System.EventHandler(this.genRemoveButton_Click);
            // 
            // genAddButton
            // 
            this.genAddButton.Location = new System.Drawing.Point(375, 19);
            this.genAddButton.Name = "genAddButton";
            this.genAddButton.Size = new System.Drawing.Size(70, 20);
            this.genAddButton.TabIndex = 8;
            this.genAddButton.Text = "Add";
            this.genAddButton.UseVisualStyleBackColor = true;
            this.genAddButton.Click += new System.EventHandler(this.genAddButton_Click);
            // 
            // genPlayerTextBox
            // 
            this.genPlayerTextBox.Location = new System.Drawing.Point(50, 19);
            this.genPlayerTextBox.Name = "genPlayerTextBox";
            this.genPlayerTextBox.Size = new System.Drawing.Size(319, 20);
            this.genPlayerTextBox.TabIndex = 7;
            this.genPlayerTextBox.Text = "Enter an username (e.g., [USERNAME])";
            // 
            // genPlayersListBox
            // 
            this.genPlayersListBox.FormattingEnabled = true;
            this.genPlayersListBox.Location = new System.Drawing.Point(50, 57);
            this.genPlayersListBox.Name = "genPlayersListBox";
            this.genPlayersListBox.Size = new System.Drawing.Size(319, 82);
            this.genPlayersListBox.TabIndex = 6;
            // 
            // genSchemeGroupBox
            // 
            this.genSchemeGroupBox.Controls.Add(this.genNNumericUpDown);
            this.genSchemeGroupBox.Controls.Add(this.genKNumericUpDown);
            this.genSchemeGroupBox.Controls.Add(this.genNLabel);
            this.genSchemeGroupBox.Controls.Add(this.genKLabel);
            this.genSchemeGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.genSchemeGroupBox.Location = new System.Drawing.Point(3, 169);
            this.genSchemeGroupBox.Name = "genSchemeGroupBox";
            this.genSchemeGroupBox.Size = new System.Drawing.Size(517, 84);
            this.genSchemeGroupBox.TabIndex = 3;
            this.genSchemeGroupBox.TabStop = false;
            this.genSchemeGroupBox.Text = "Set threshold scheme";
            // 
            // genNNumericUpDown
            // 
            this.genNNumericUpDown.Location = new System.Drawing.Point(216, 51);
            this.genNNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.genNNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.genNNumericUpDown.Name = "genNNumericUpDown";
            this.genNNumericUpDown.Size = new System.Drawing.Size(82, 20);
            this.genNNumericUpDown.TabIndex = 5;
            this.genNNumericUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // genKNumericUpDown
            // 
            this.genKNumericUpDown.Location = new System.Drawing.Point(216, 20);
            this.genKNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.genKNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.genKNumericUpDown.Name = "genKNumericUpDown";
            this.genKNumericUpDown.Size = new System.Drawing.Size(82, 20);
            this.genKNumericUpDown.TabIndex = 4;
            this.genKNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // genNLabel
            // 
            this.genNLabel.AutoSize = true;
            this.genNLabel.Location = new System.Drawing.Point(99, 53);
            this.genNLabel.Name = "genNLabel";
            this.genNLabel.Size = new System.Drawing.Size(113, 13);
            this.genNLabel.TabIndex = 3;
            this.genNLabel.Text = "Number of players (n) :";
            // 
            // genKLabel
            // 
            this.genKLabel.AutoSize = true;
            this.genKLabel.Location = new System.Drawing.Point(99, 22);
            this.genKLabel.Name = "genKLabel";
            this.genKLabel.Size = new System.Drawing.Size(111, 13);
            this.genKLabel.TabIndex = 2;
            this.genKLabel.Text = "Threshold             (k) :";
            // 
            // genSecretGroupBox
            // 
            this.genSecretGroupBox.Controls.Add(this.label1);
            this.genSecretGroupBox.Controls.Add(this.genTextBox);
            this.genSecretGroupBox.Controls.Add(this.genFileTextBox);
            this.genSecretGroupBox.Controls.Add(this.genBrowseButton);
            this.genSecretGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.genSecretGroupBox.Location = new System.Drawing.Point(3, 55);
            this.genSecretGroupBox.Name = "genSecretGroupBox";
            this.genSecretGroupBox.Size = new System.Drawing.Size(517, 114);
            this.genSecretGroupBox.TabIndex = 1;
            this.genSecretGroupBox.TabStop = false;
            this.genSecretGroupBox.Text = "Input your secret";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(253, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "or";
            // 
            // genTextBox
            // 
            this.genTextBox.Location = new System.Drawing.Point(50, 19);
            this.genTextBox.Multiline = true;
            this.genTextBox.Name = "genTextBox";
            this.genTextBox.Size = new System.Drawing.Size(406, 52);
            this.genTextBox.TabIndex = 1;
            this.genTextBox.Text = "Enter a text...";
            // 
            // genFileTextBox
            // 
            this.genFileTextBox.Location = new System.Drawing.Point(50, 92);
            this.genFileTextBox.Name = "genFileTextBox";
            this.genFileTextBox.Size = new System.Drawing.Size(323, 20);
            this.genFileTextBox.TabIndex = 2;
            this.genFileTextBox.Text = "Select a file";
            // 
            // genBrowseButton
            // 
            this.genBrowseButton.Location = new System.Drawing.Point(379, 92);
            this.genBrowseButton.Name = "genBrowseButton";
            this.genBrowseButton.Size = new System.Drawing.Size(77, 20);
            this.genBrowseButton.TabIndex = 3;
            this.genBrowseButton.Text = "Browse";
            this.genBrowseButton.UseVisualStyleBackColor = true;
            this.genBrowseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // genNameGroupBox
            // 
            this.genNameGroupBox.Controls.Add(this.genNameTextBox);
            this.genNameGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.genNameGroupBox.Location = new System.Drawing.Point(3, 3);
            this.genNameGroupBox.Name = "genNameGroupBox";
            this.genNameGroupBox.Size = new System.Drawing.Size(517, 52);
            this.genNameGroupBox.TabIndex = 0;
            this.genNameGroupBox.TabStop = false;
            this.genNameGroupBox.Text = "Specify scheme name";
            // 
            // genNameTextBox
            // 
            this.genNameTextBox.Location = new System.Drawing.Point(50, 19);
            this.genNameTextBox.Name = "genNameTextBox";
            this.genNameTextBox.Size = new System.Drawing.Size(319, 20);
            this.genNameTextBox.TabIndex = 0;
            this.genNameTextBox.Text = "Enter a name";
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
            this.generateTabPage.ResumeLayout(false);
            this.genStatusPanel.ResumeLayout(false);
            this.genStatusPanel.PerformLayout();
            this.genPlayersGroupBox.ResumeLayout(false);
            this.genPlayersGroupBox.PerformLayout();
            this.genSchemeGroupBox.ResumeLayout(false);
            this.genSchemeGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.genNNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.genKNumericUpDown)).EndInit();
            this.genSecretGroupBox.ResumeLayout(false);
            this.genSecretGroupBox.PerformLayout();
            this.genNameGroupBox.ResumeLayout(false);
            this.genNameGroupBox.PerformLayout();
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
        private System.Windows.Forms.TextBox genFileTextBox;
        private System.Windows.Forms.Button genBrowseButton;
        private System.Windows.Forms.OpenFileDialog genOpenFileDialog;
        private System.Windows.Forms.GroupBox genSecretGroupBox;
        private System.Windows.Forms.TextBox genTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox genSchemeGroupBox;
        private System.Windows.Forms.Label genNLabel;
        private System.Windows.Forms.Label genKLabel;
        private System.Windows.Forms.NumericUpDown genNNumericUpDown;
        private System.Windows.Forms.NumericUpDown genKNumericUpDown;
        private System.Windows.Forms.ListBox genPlayersListBox;
        private System.Windows.Forms.GroupBox genNameGroupBox;
        private System.Windows.Forms.TextBox genNameTextBox;
        private System.Windows.Forms.GroupBox genPlayersGroupBox;
        private System.Windows.Forms.Button genShareButton;
        private System.Windows.Forms.Button genRemoveButton;
        private System.Windows.Forms.Button genAddButton;
        private System.Windows.Forms.TextBox genPlayerTextBox;
        private System.Windows.Forms.Panel genStatusPanel;
        private System.Windows.Forms.Label genStatusLabel;
        private System.Windows.Forms.Label genDontCloseLabel;
    }
}