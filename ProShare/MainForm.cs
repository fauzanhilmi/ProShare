using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace ProShare
{
    public partial class MainForm : Form
    {
        private string username;

        /*      GENERATE attributes         */
        private string genEmptyName = "Enter a name";
        private string genEmptyText = "Enter a text here...";
        private string genEmptyFile = "Select a file";
        private string genEmptyPlayer = "Enter an username (e.g., [USERNAME])"; //NANTI GANTI PAKE USERNAME!!
        private string genPlayersCount1 = "Add ";
        private string genPlayersCount2 = " players";

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string name)
        {
            username = name;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = ColorTranslator.FromHtml("#00B0F0");

            /*              GENERATE initializations            */
            genSecretGroupBox.Visible = false; //MIGHT BE changed??

            genNameTextBox.Text = genEmptyName;
            genNameTextBox.TextChanged += GenNameTextBox_TextChanged;
            genNameTextBox.GotFocus += GenNameTextBox_GotFocus;
            genNameTextBox.LostFocus += GenNameTextBox_LostFocus;

            genTextBox.Text = genEmptyText;
            genFileTextBox.Text = genEmptyFile;
            genTextBox.GotFocus += GenTextBox_GotFocus;
            genTextBox.LostFocus += GenTextBox_LostFocus;
            genFileTextBox.GotFocus += GenFileTextBox_GotFocus;
            genFileTextBox.LostFocus += GenFileTextBox_LostFocus;

            genKNumericUpDown.ValueChanged += GenKNumericUpDown_ValueChanged;
            genNNumericUpDown.ValueChanged += GenNNumericUpDown_ValueChanged;

            int numPlayersLeft = (int)genNNumericUpDown.Value - genPlayersListBox.Items.Count;
            genPlayersGroupBox.Text = genPlayersCount1 + numPlayersLeft.ToString() + genPlayersCount2;
            genRemoveButton.Enabled = false;
            genPlayersGroupBox.TextChanged += GenPlayersGroupBox_TextChanged;
            genPlayerTextBox.KeyDown += GenPlayerTextBox_KeyDown;
            genPlayerTextBox.GotFocus += GenPlayerTextBox_GotFocus;
            genPlayerTextBox.LostFocus += GenPlayerTextBox_LostFocus;
            genPlayersListBox.SelectedIndexChanged += GenPlayersListBox_SelectedIndexChanged;
            genPlayersListBox.KeyDown += GenPlayersListBox_KeyDown;

            genShareButton.Enabled = false;

        }

        private void operationsButton_Click(object sender, EventArgs e)
        {
            stackPanel.SelectTab("Operations");
        }

        private void requestsButton_Click(object sender, EventArgs e)
        {
            stackPanel.SelectTab("Requests");
        }

        /*          GENERATE methods            */
        private void GenNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(genNameTextBox.Text) && genNameTextBox.Text != genEmptyName)
            {
                int numPlayersLeft = (int)genNNumericUpDown.Value - genPlayersListBox.Items.Count;
                if (numPlayersLeft == 0)
                {
                    genShareButton.Enabled = true;
                }
            }
            else
            {
                genShareButton.Enabled = false;
            }
        }

        private void GenNameTextBox_GotFocus(object sender, EventArgs e)
        {
            if (genNameTextBox.Text == genEmptyName)
            {
                genNameTextBox.Text = "";
            }
        }

        private void GenNameTextBox_LostFocus(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(genNameTextBox.Text) || genNameTextBox.Text == genEmptyName)
            {
                genNameTextBox.Text = genEmptyName;
                genShareButton.Enabled = false;
            }
            else
            {
                int numPlayersLeft = (int)genNNumericUpDown.Value - genPlayersListBox.Items.Count;
                if (numPlayersLeft == 0)
                {
                    genShareButton.Enabled = true;
                }
            }
        }

        private void GenTextBox_GotFocus(object sender, EventArgs e)
        {
            if(genTextBox.Text == genEmptyText)
            {
                genTextBox.Text = "";
            }
        }

        private void GenTextBox_LostFocus(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(genTextBox.Text) || genTextBox.Text == genEmptyText)
            {
                genTextBox.Text = genEmptyText;
                genFileTextBox.ReadOnly = false;
                //genFileTextBox.Enabled = true;
                genBrowseButton.Enabled = true;
            }
            else
            {
                genFileTextBox.ReadOnly = true;
                //genFileTextBox.Enabled = false;
                genBrowseButton.Enabled = false;
            }
        }

        private void GenFileTextBox_GotFocus(object sender, EventArgs e)
        {
            if(genFileTextBox.Text == genEmptyFile)
            {
                genFileTextBox.Text = "";
            }
        }

        private void GenFileTextBox_LostFocus(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(genFileTextBox.Text) || genFileTextBox.Text == genEmptyFile)
            {
                genFileTextBox.Text = genEmptyFile;
                //genTextBox.Enabled = true;
                genTextBox.ReadOnly = false;
            }
            else if(isFileOrDirectoryExists(genFileTextBox.Text))
            {
                //genTextBox.Enabled = false;
                genTextBox.ReadOnly = true;
            }
            else //path is not valid
            {
                MessageBox.Show("Please enter a valid file path", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                genFileTextBox.Text = genEmptyFile;
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = genOpenFileDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                string fileLocation = genOpenFileDialog.FileName;
                genFileTextBox.Text = fileLocation;
                //genTextBox.Enabled = false;
                genTextBox.ReadOnly = true;
            }          
        }

        private void GenKNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (genKNumericUpDown.Value > genNNumericUpDown.Value)
            {
                genNNumericUpDown.Value = genKNumericUpDown.Value;
            }
        }

        private void GenNNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (genKNumericUpDown.Value > genNNumericUpDown.Value)
            {
                genKNumericUpDown.Value = genNNumericUpDown.Value;
            }
            int numPlayersLeft = (int)genNNumericUpDown.Value - genPlayersListBox.Items.Count;
            genPlayersGroupBox.Text = genPlayersCount1 + numPlayersLeft.ToString() + genPlayersCount2;
            if(numPlayersLeft > 0)
            {
                genAddButton.Enabled = true;
            }
            else if(numPlayersLeft <= 0)
            {
                genAddButton.Enabled = false;
            }
        }

        private void GenPlayersGroupBox_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(genNameTextBox.Text) && genNameTextBox.Text != genEmptyName)
            {
                int numPlayersLeft = (int)genNNumericUpDown.Value - genPlayersListBox.Items.Count;
                if (numPlayersLeft == 0)
                {
                    genShareButton.Enabled = true;
                }
                else
                {
                    genShareButton.Enabled = false;
                }
            }
        }

        private void GenPlayerTextBox_GotFocus(object sender, EventArgs e)
        {
            if (genPlayerTextBox.Text == genEmptyPlayer)
            {
                genPlayerTextBox.Text = "";
            }
        }

        private void GenPlayerTextBox_LostFocus(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(genPlayerTextBox.Text) || genPlayerTextBox.Text == genEmptyPlayer)
            {
                genPlayerTextBox.Text = genEmptyPlayer;
            }
        }

        private void GenPlayerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                genAddButton.PerformClick();
            }
        }

        private void genAddButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(genPlayerTextBox.Text) || genPlayerTextBox.Text == genEmptyPlayer)
            {
                MessageBox.Show("You typed empty username", "Cannot Add Player", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                int index = genPlayersListBox.FindStringExact(genPlayerTextBox.Text);
                if(index != -1) //player is already added
                {
                    MessageBox.Show("You have already added this username", "Cannot Add Player", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    genPlayersListBox.Items.Add(genPlayerTextBox.Text);
                    int numPlayersLeft = (int)genNNumericUpDown.Value - genPlayersListBox.Items.Count;
                    genPlayersGroupBox.Text = genPlayersCount1 + numPlayersLeft.ToString() + genPlayersCount2;
                    if(numPlayersLeft == 0)
                    {
                        genAddButton.Enabled = false;
                        genNNumericUpDown.Minimum = genNNumericUpDown.Value;
                    }
                }
            }
        }

        private void GenPlayersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            genRemoveButton.Enabled = true;
        }

        private void GenPlayersListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back)
            {
                genRemoveButton.PerformClick();
            }
        }

        private void genRemoveButton_Click(object sender, EventArgs e)
        {
            genPlayersListBox.Items.Remove(genPlayersListBox.SelectedItem);
            genRemoveButton.Enabled = false;

            if (genPlayersListBox.Items.Count < genNNumericUpDown.Minimum && genPlayersListBox.Items.Count > 0)
            {
                genNNumericUpDown.Minimum = genPlayersListBox.Items.Count;
            }

            int numPlayersLeft = (int)genNNumericUpDown.Value - genPlayersListBox.Items.Count;
            genPlayersGroupBox.Text = genPlayersCount1 + numPlayersLeft.ToString() + genPlayersCount2;
            if(numPlayersLeft > 0)
            {
                genAddButton.Enabled = true;
            }
        }

        private void genShareButton_Click(object sender, EventArgs e)
        {

        }

        /*      Other Methods       */
        internal static bool isFileOrDirectoryExists(string name)
        {
            return (Directory.Exists(name) || File.Exists(name));
        }
    }
}
