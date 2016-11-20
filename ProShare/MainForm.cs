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
using System.Threading;
using System.Reflection;

namespace ProShare
{
    public partial class MainForm : Form
    {
        /*      MainForm attributes         */
        //private string defaultNotificationsText = "Notifications ("; //minus the right side!
        private string username;
        private IDictionary<ulong, IDictionary<string, object>> ntfDictionary; //dictionary of notifications

        private delegate void SetTextCallback(Control obj, string text); //for changing control object's text when notification (MQ message) arrived
        private delegate void AddNtfButtonCallback(ulong DeliveryTag, IDictionary<string, object> contents);

        /*      GENERATE attributes         */
        private const string genEmptyName = "Enter a name";
        private const string genEmptyText = "Enter a text here...";
        private const string genEmptyFile = "Select a file";
        private const string genPlayersCount1 = "Add ";
        private const string genPlayersCount2 = " players";
        private string genEmptyPlayer;
        private const string genEmptyPlayer1 = "Enter an username (e.g., ";
        private const string genEmptyPlayer2 = ")";
        private const string genFirstStatus = "Sending share requests...";

        /*      RECONSTRUCT attributes      */
        private const string recEmptySchemeText = "You are not involved in any scheme";
        private const string recEmptyFile = "Select a file";
        private string[] recShareFiles;

        /*      UPDATE attributes           */
        private const string updEmptySchemeText = "You are not involved in any scheme(s)";
        private const string updEmptyShare = "Select a share file";
        private const string updEmptySubshare = "Select subhsare files";

        /*      Notifications attributes         */
        private const int numOfNotifications = 0;
        private const string browseEmptyText = "Enter a text...";
        private const string browseEmptyFile = "Select files";
        //private byte[] current_bytes;
        //private byte current_k; //for Generate - Dealer
        //private byte current_n;
        //private string ntftText = "Notifications (";


        /*      TEST attributes                  */
        //private string current_scheme;

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
            /*              MainForm initializations            */
            MQHandler.Connect();
            //TEST
            //username = "fauzan";

            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = ColorTranslator.FromHtml("#00B0F0");

            menuGroupBox.Text = "Hello, " + username + "!";
            //notificationsButton.Text = defaultNotificationsText + "0)";
            //notificationsButton.Text = ntfLeftText + numOfNotifications + ")";

            this.FormClosing += MainForm_FormClosing;

            MQHandler.GetMessage(username, ProcessNotification); //Listening to incoming notifications
            ntfDictionary = new Dictionary<ulong, IDictionary<string, object>>();

            /*              General initializations             */
            operationsTabControl.SelectedIndexChanged += OperationsTabControl_SelectedIndexChanged;

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

            genEmptyPlayer = genEmptyPlayer1 + username + genEmptyPlayer2;
            genPlayerTextBox.Text = genEmptyPlayer;
            int numPlayersLeft = (int)genNNumericUpDown.Value - genPlayersListBox.Items.Count;
            genPlayersGroupBox.Text = genPlayersCount1 + numPlayersLeft.ToString() + genPlayersCount2;
            genRemoveButton.Enabled = false;
            genPlayersGroupBox.TextChanged += GenPlayersGroupBox_TextChanged;
            genPlayerTextBox.KeyDown += GenPlayerTextBox_KeyDown;
            genPlayerTextBox.GotFocus += GenPlayerTextBox_GotFocus;
            genPlayerTextBox.LostFocus += GenPlayerTextBox_LostFocus;
            genPlayersListBox.SelectedIndexChanged += GenPlayersListBox_SelectedIndexChanged;
            genPlayersListBox.KeyDown += GenPlayersListBox_KeyDown;

            genStatusLabel.Text = genFirstStatus;
            genStatusLabel.Visible = false;
            genDontCloseLabel.Visible = false;
            genShareButton.Enabled = false;

            /*              Notification initializations            */
            ntfPanel.AutoScroll = true;
            ntfConfLabel1.Visible = false;
            ntfConfLabel2.Visible = false;
            ntfConfButton1.Visible = false;
            ntfConfButton2.Visible = false;
            browseSecretTextBox.Text = browseEmptyText;
            browseGenerateButton.Enabled = true;
        }

        private void OperationsTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show((sender as TabControl).SelectedIndex.ToString());
            switch((sender as TabControl).SelectedIndex)
            {
                case 0: //share generation
                    {
                        break;
                    }
                case 1: //secret reconstruction
                    {
                        List<string> schemes = new List<string>();
                        try
                        {
                            DatabaseHandler.Connect();
                            schemes = DatabaseHandler.GetSchemesByPlayer(username);
                            DatabaseHandler.Close();
                        }
                        catch (MySql.Data.MySqlClient.MySqlException ex)
                        {
                            MessageBox.Show(ex.Message, "Unexpected " + ex.Number + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (schemes.Count == 0)
                        {
                            recSchemesComboBox.Text = recEmptySchemeText;
                            recSendButton.Enabled = false;
                            recSchemesComboBox2.Text = recEmptySchemeText;
                            recGenerateButton.Enabled = false;
                        }
                        else
                        {
                            BindingSource bs = new BindingSource();
                            bs.DataSource = schemes;
                            recSchemesComboBox.DataSource = bs;
                            recSendButton.Enabled = true;
                            BindingSource bs2 = new BindingSource();
                            bs2.DataSource = schemes;
                            recSchemesComboBox2.DataSource = bs2;
                            recGenerateButton.Enabled = true;
                        }
                        break;
                    }
                case 2: //share update
                    {
                        List<string> schemes = new List<string>();
                        try
                        {
                            DatabaseHandler.Connect();
                            schemes = DatabaseHandler.GetSchemesByPlayer(username);
                            DatabaseHandler.Close();
                        }
                        catch (MySql.Data.MySqlClient.MySqlException ex)
                        {
                            MessageBox.Show(ex.Message, "Unexpected " + ex.Number + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (schemes.Count == 0)
                        {
                            updSendSchemesComboBox.Text = updEmptySchemeText;
                            updSendButton.Enabled = false;
                            updGenerateSchemesComboBox.Text = updEmptySchemeText;
                            updGenerateButton.Enabled = false;
                        }
                        else
                        {
                            BindingSource bs = new BindingSource();
                            bs.DataSource = schemes;
                            updSendSchemesComboBox.DataSource = bs;
                            updSendButton.Enabled = true;
                            BindingSource bs2 = new BindingSource();
                            bs2.DataSource = schemes;
                            updGenerateSchemesComboBox.DataSource = bs;
                            updGenerateButton.Enabled = true;
                        }
                        break;
                    }
            }
        }

        private void operationsButton_Click(object sender, EventArgs e)
        {
            //stackPanel.SelectTab("Operations");
            stackPanel.SelectTab(0);
        }

        private void requestsButton_Click(object sender, EventArgs e)
        {
            //stackPanel.SelectTab("Notifications");
            stackPanel.SelectTab(1);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MQHandler.Close();
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
            //else if(isFileOrDirectoryExists(genFileTextBox.Text)) //it isn't working on Notification's "browse"
            else if(File.Exists(genFileTextBox.Text))
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

        private void browseButton_Click(object sender, EventArgs e) //browseBrowseButton
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
                        genPlayerTextBox.Text = "";
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
            string schemeName = genNameTextBox.Text;
            byte k = (byte)genKNumericUpDown.Value;
            byte n = (byte)genNNumericUpDown.Value;
            List<string> players = genPlayersListBox.Items.Cast<string>().ToList();
            try
            {
                DatabaseHandler.Connect();
                try
                {
                    int checkRes = DatabaseHandler.DoAccountsExist(players);
                    if(checkRes == 1)
                    {
                        DatabaseHandler.Close(); 
                        DatabaseHandler.Connect(); //reopen SQL connection
                        int addRes = DatabaseHandler.AddScheme(schemeName, username, k, n); //try to add scheme
                        if (addRes == 1) //success
                        {
                            //genStatusLabel.Text = genFirstStatus;
                            genStatusLabel.Visible = true;
                            genDontCloseLabel.Visible = true;

                            DatabaseHandler.AddPlayers(schemeName, players); //add players to db
                            //TEST
                            //MQHandler.Connect();
                            /* foreach(string player in players)
                            {
                                //MQHandler.SendShareRequest(schemeName, username, player);
                            }*/
                            //MQHandler.Close();

                            MQHandler.SendShareRequests(schemeName, username, players);
                            
                            MessageBox.Show("Share requests were sent sucessfully", "Share Requests Delivery Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            genStatusLabel.Visible = false;
                            //genStatusLabel.Text = genFirstStatus;
                            genDontCloseLabel.Visible = false;
                        }
                        else if (addRes == 0) //failed, scheme already exists
                        {
                            MessageBox.Show("Scheme with that name already exists. Please choose different name.", "Share Requests Delivery Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        //else catch exception
                    }
                    else if(checkRes == 0) //One or more player does not exist
                    {
                        MessageBox.Show("A player's name(s) does not exist. Please check it again.", "Share Requests Delivery Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }                    
                    //else catch exception
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Unexpected " + ex.Number + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                DatabaseHandler.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*          RECONSTRUCT Methods             */
        private void recSendButton_Click(object sender, EventArgs e)
        {
            if (recSchemesComboBox.Text == recEmptySchemeText)
            {
                //do nothing
            }
            else
            {
                string scheme = recSchemesComboBox.Text;
                try
                {
                    DatabaseHandler.Connect();
                    DatabaseHandler.ResetConfirmations(scheme);
                    DatabaseHandler.Close();

                    MQHandler.SendFanoutMessages("Reconstruct", "Request", scheme, username, ASCIIEncoding.ASCII.GetBytes(""));
                    MessageBox.Show("Reconstruction requests delivery is completed", "Delivery Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Unexpected " + ex.Number + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void recBrowseButton_Click(object sender, EventArgs e)
        {
            recOpenFileDialog.Multiselect = true;
            DialogResult result = recOpenFileDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                recShareFiles = recOpenFileDialog.FileNames;
                string oneline = "";
                foreach(string file in recShareFiles)
                {
                    string current = "\"" + Path.GetFileName(file) + "\"";
                    oneline += current;
                }
                recSharesTextBox.Text = oneline;
            }
            else
            {
                recSharesTextBox.Text = recEmptyFile;
            }
        }

        private void recGenerateButton_Click(object sender, EventArgs e)
        {
            string scheme = recSchemesComboBox2.Text;
            if (scheme == recEmptySchemeText)
            {
                //do nothing
            }
            else
            {
                if(recSharesTextBox.Text != recEmptyFile)
                {
                    DialogResult dr = recSaveFileDialog.ShowDialog();
                    if(dr == DialogResult.OK)
                    {
                        try
                        {
                            DatabaseHandler.Connect();
                            List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                            byte k = (byte)(ulong)schemeInfos[3];
                            byte n = (byte)(ulong)schemeInfos[4];
                            SecretSharing.ReconstructFileSecret(recShareFiles, k, recSaveFileDialog.FileName);
                            //File.WriteAllBytes(filename, current_bytes);
                            MessageBox.Show("Secret reconstruction process is completed", "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (MySql.Data.MySqlClient.MySqlException ex)
                        {
                            Debug.WriteLine(ex.Number + " " + ex.Message);
                        }
                    }
                }
            }
        }

        /*          UPDATE methods                  */
        private void updSendButton_Click(object sender, EventArgs e)
        {
            if(updSendSchemesComboBox.Text == updEmptySchemeText)
            {
                //do nothing
            }
            else
            {
                string scheme = updSendSchemesComboBox.Text;
                try
                {
                    DatabaseHandler.Connect();
                    DatabaseHandler.ResetConfirmations(scheme);
                    DatabaseHandler.Close();

                    MQHandler.SendFanoutMessages("Update", "Request", scheme, username, ASCIIEncoding.ASCII.GetBytes(""));
                    MessageBox.Show("Update requests delivery is completed", "Delivery Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Unexpected " + ex.Number + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        /*          Notification methods            */
        private void ProcessNotification(ulong DeliveryTag, IDictionary<string, object> headers, byte[] message)
        {
            IDictionary<string, object> contents = new Dictionary<string, object>(headers);
            contents.Add("Message", message);
            ntfDictionary.Add(DeliveryTag, contents);

            //IncrementNotifications();
            AddNtfButton(DeliveryTag, contents);

            //TEST
            /*Debug.WriteLine(">" + DeliveryTag);
            foreach(var item in contents)
            {
                Debug.WriteLine(item.Key + " : " + Encoding.ASCII.GetString((byte[]) item.Value) + " (" + item.Value.ToString() + ")");
            }
            Debug.WriteLine("");*/
        }

        /*private void IncrementNotifications()
        {
            //HANDLE CASE KALO 2 DIGIT!!
            //TEST
            //ulong numOfNotifications = (ulong)notificationsButton.Text[15] - '0'; //change index if the text is changed!
            //numOfNotifications++;
            numOfNotifications++;
            string newNotificationsText = ntfLeftText + numOfNotifications + ")";
            //TEST
            SetText(notificationsButton, newNotificationsText);
            Debug.WriteLine(notificationsButton.Text);
        }

        private void DecrementNotifications()
        {
            //HANDLE CASE KALO 2 DIGIT!!
            //ulong numOfNotifications = (ulong)notificationsButton.Text[15] - '0'; //change index if the text is changed!
            numOfNotifications--;
            //string newNotificationsText = defaultNotificationsText + numOfNotifications + ")";
            string newNotificationsText = ntfLeftText + numOfNotifications + ")";
            //TEST
            SetText(notificationsButton, newNotificationsText);
            Debug.WriteLine(notificationsButton.Text);
            //notificationsButton.Text = newNotificationsText;
        }*/

        private void SetText(Control obj, string text)
        {
            if (obj.InvokeRequired)
            {
                SetTextCallback stc = new SetTextCallback(SetText);
                //this.BeginInvoke(stc, new object[] { obj, text });
                this.Invoke(stc, new object[] { obj, text });
            }
            else
            {
                obj.Text = text;
            }
        }

        //GANTI NAMA!
        private void AddNtfButton(ulong DeliveryTag, IDictionary<string, object> contents)
        {
            Button ntfButton = new Button();
            ntfButton.Tag = DeliveryTag;
            ntfButton.Name = "ntfButton" + DeliveryTag;
            ntfButton.BackColor = SystemColors.GradientInactiveCaption;
            ntfButton.FlatStyle = FlatStyle.Flat;
            ntfButton.FlatAppearance.BorderColor = SystemColors.ScrollBar;
            ntfButton.Dock = DockStyle.Top;
            ntfButton.TextAlign = ContentAlignment.TopLeft;

            //setting the button text
            string operation = Encoding.ASCII.GetString((byte[])contents["Operation"]);
            string type = Encoding.ASCII.GetString((byte[])contents["Type"]);
            string scheme = Encoding.ASCII.GetString((byte[])contents["Scheme"]);
            //current_scheme = scheme;
            string sender = Encoding.ASCII.GetString((byte[])contents["Sender"]);
            byte[] message = (byte[])contents["Message"];

            //Debug.WriteLine(username + " received (" + operation + ", " + type + ", " + scheme + ", " + sender + ", " +message+")");

            switch(operation)
            {
                case "Generate":
                    {
                        switch(type)
                        {
                            case "Request":
                                {
                                    ntfButton.Text = "(Share Request) " + sender + " invites you to join scheme '" + scheme + "'";
                                    ntfButton.Click += (o, e) =>
                                    {
                                        ntfActionStackPanel.SelectTab(0); //Action tabpage
                                        ntfConfLabel1.Text = sender + " wants you to join his secret sharing scheme '" + scheme + "' as a player.";
                                        ntfConfLabel1.Visible = true;
                                        ntfConfLabel2.Text = "Click 'Accept' to accept this request. Otherwise, click 'Reject'.";
                                        ntfConfLabel2.Visible = true;

                                        ntfConfButton1.Text = "Accept";
                                        ntfConfButton1.Visible = true;
                                        RemoveClickEvent(ntfConfButton1);
                                        ntfConfButton1.Click += (o1, e1) => //If scheme is already deleted, it does nothing
                                        {
                                            try
                                            {
                                                DatabaseHandler.Connect();
                                                DatabaseHandler.IncrementConfirmations(scheme);

                                                MQHandler.Ack(DeliveryTag);
                                                MQHandler.SendDirectMessage("Generate", "Response", scheme, username, sender, BitConverter.GetBytes(true)); //here sender = destination!

                                                List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                                                string dealer = (string)schemeInfos[2];
                                                ulong n = (ulong)schemeInfos[4];
                                                ulong num_of_confirmations = (ulong)schemeInfos[5];
                                                //Send notifications if all players have been accepted
                                                if (num_of_confirmations == n)
                                                {
                                                    MQHandler.SendFanoutMessages("Generate", "Notice", scheme, dealer, BitConverter.GetBytes(true));
                                                    MQHandler.SendDirectMessage("Generate", "Dealer", scheme, "System", dealer, ASCIIEncoding.ASCII.GetBytes("")); //send special request to dealer
                                                }

                                                ntfConfLabel2.Text = "You accepted this request.";
                                                ntfConfButton1.Visible = false;
                                                ntfConfButton2.Visible = false;
                                                ntfPanel.Controls.Remove(ntfButton);
                                                ntfButton.Dispose();
                                                //DecrementNotifications(); //Why it isn't working :(
                                                DatabaseHandler.Close();
                                            }
                                            catch (MySql.Data.MySqlClient.MySqlException ex)
                                            {
                                                Debug.WriteLine(ex.Number + " : " + ex.Message);
                                                MessageBox.Show("Something went wrong. Please try again");
                                            }
                                        };

                                        ntfConfButton2.Text = "Reject";
                                        ntfConfButton2.Visible = true;
                                        RemoveClickEvent(ntfConfButton2);
                                        ntfConfButton2.Click += (o2, e2) => //If scheme already deleted, it does nothing
                                        {

                                            MQHandler.Ack(DeliveryTag);
                                            MQHandler.SendDirectMessage("Generate", "Response", scheme, username, sender, BitConverter.GetBytes(false)); //here sender = destination!

                                            ntfConfLabel2.Text = "You rejected this request.";
                                            ntfConfButton1.Visible = false;
                                            ntfConfButton2.Visible = false;
                                            ntfPanel.Controls.Remove(ntfButton);
                                            ntfButton.Dispose();

                                            //DecrementNotifications(); //Why it isn't working :(
                                        };
                                    };
                                    break;
                                }
                            case "Response": //Only dealer who gets this
                                {
                                    bool isAccepted = BitConverter.ToBoolean(message, 0);
                                    string response = "";
                                    if(isAccepted)
                                    {
                                        response = "accepted";
                                    }
                                    else
                                    {
                                        response = "rejected";;
                                    }
                                    ntfButton.Text = "[" + scheme + "] " + sender + " " + response + " your share request";
                                    ntfButton.Click += (o, e) =>
                                    {
                                        MQHandler.Ack(DeliveryTag);

                                        ntfActionStackPanel.SelectTab(0); //Action tabpage
                                        ntfConfLabel1.Text = sender + " has " + response + " your request to join scheme '" + scheme + "' as player";
                                        ntfConfLabel1.Visible = true;
                                        if(isAccepted) //kalo udah kehapus gimana???
                                        {
                                            try
                                            {
                                                DatabaseHandler.Connect();
                                                List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                                                ulong n = (ulong)schemeInfos[4];
                                                ulong num_of_confirmations = (ulong)schemeInfos[5];
                                                ntfConfLabel2.Text = "Number of confirmations so far : " + num_of_confirmations + "/" + n;
                                                ntfConfLabel2.Visible = true;
                                                DatabaseHandler.Close();
                                            }
                                            catch (MySql.Data.MySqlClient.MySqlException ex)
                                            {
                                                Debug.WriteLine("Something goes wrong on Database");
                                            }
                                        }
                                        else
                                        {
                                            ntfConfLabel2.Text = "This means the scheme '" + scheme + "' cannot be used and now will be deleted.";
                                            ntfConfLabel2.Visible = true;
                                            //Send deletion notice
                                            MQHandler.SendFanoutMessages("Generate", "Notice", scheme, username, BitConverter.GetBytes(false));

                                            //Deleting scheme in db & mq
                                            try
                                            {
                                                DatabaseHandler.Connect();
                                                DatabaseHandler.DeleteScheme(scheme);
                                                DatabaseHandler.Close();
                                            }
                                            catch (MySql.Data.MySqlClient.MySqlException ex)
                                            {
                                                Debug.WriteLine(ex.Number + " : " + ex.Message);
                                                MessageBox.Show("Something went wrong. Please try again");
                                            }
                                            //Thread.Sleep(5000);
                                            //MQHandler.DeleteExchange(scheme); //Harusnya dihapus pas semua udah baca notif failure
                                        }
                                        ntfConfButton1.Visible = false;
                                        ntfConfButton2.Visible = false;
                                        ntfPanel.Controls.Remove(ntfButton);
                                        ntfButton.Dispose();
                                    };
                                    break;
                                }
                            case "Notice": //All players gets this
                                {
                                    bool isContinued = BitConverter.ToBoolean(message, 0);
                                    string bText = "", label1Text = "", label2Text = "";
                                    if(isContinued)
                                    {
                                        bText = "[" + scheme + "] " + " Scheme advances to share distribution";
                                        label1Text = "Good news, all players have accepted the share requests!";
                                        label2Text = "Now just wait for the dealer (" + sender + ") to distributes the shares.";
                                    }
                                    else
                                    {
                                        bText = "[" + scheme + "] " + " Scheme fails to advance to share distribution";
                                        label1Text = "Bad news, one or more player(s) have rejected the share request.";
                                        label2Text = "Now the scheme '" + scheme + "'cannot be used and will be deleted.";
                                    }

                                    ntfButton.Text = bText;
                                    ntfButton.Click += (o, e) =>
                                    {
                                        MQHandler.Ack(DeliveryTag);

                                        ntfActionStackPanel.SelectTab(0); //Action
                                        ntfConfLabel1.Text = label1Text;
                                        ntfConfLabel1.Visible = true;
                                        ntfConfLabel2.Text = label2Text;
                                        ntfConfLabel2.Visible = true;
                                        ntfConfButton1.Visible = false;
                                        ntfConfButton2.Visible = false;
                                        ntfPanel.Controls.Remove(ntfButton);
                                        ntfButton.Dispose();
                                    };

                                    break;
                                }
                            case "Dealer": //Only dealer gets this after all accepted
                                {
                                    ntfButton.Text = "[" + scheme + "] Choose secret";
                                    ntfButton.Click += (o, e) =>
                                    {
                                        //MQHandler.Ack(DeliveryTag); Kirim ACK Pas udah generate aja!
                                        ntfActionStackPanel.SelectTab(1); //Browse page

                                        browseSecretTextBox.Visible = true;
                                        //BELUM DI REMOVE EVENT!
                                        browseSecretTextBox.GotFocus += (o2, e2) =>
                                        {
                                            if (browseSecretTextBox.Text == browseEmptyText)
                                            {
                                                browseSecretTextBox.Text = "";
                                            }
                                        };
                                        //BELUM DI REMOVE EVENT!
                                        browseSecretTextBox.LostFocus += (o2, e2) =>
                                        {
                                            if (String.IsNullOrWhiteSpace(browseSecretTextBox.Text) || browseSecretTextBox.Text == browseEmptyText)
                                            {
                                                browseSecretTextBox.Text = browseEmptyText;
                                                //uncomment gak?
                                                //browseBrowseButton.Enabled = false;
                                                //browseSecretFileTextBox.ReadOnly = false;
                                                //browseGenerateButton.Enabled = true;
                                            }
                                            else
                                            {
                                                //browseBrowseButton.Enabled = true;
                                                //browseSecretFileTextBox.ReadOnly = true;
                                                //browseGenerateButton.Enabled = false;
                                            }
                                        };
                                        browseSecretFileTextBox.Visible = true;
                                        //BELUM DI REMOVE EVENT!
                                        browseSecretFileTextBox.GotFocus += (o2, e2) =>
                                        {
                                            if (browseSecretFileTextBox.Text == browseEmptyFile)
                                            {
                                                browseSecretFileTextBox.Text = "";
                                            }
                                        };
                                        //BELUM DI REMOVE EVENT!
                                        browseSecretFileTextBox.LostFocus += (o2, e2) =>
                                        {
                                            if (String.IsNullOrWhiteSpace(browseSecretFileTextBox.Text) || browseSecretFileTextBox.Text == browseEmptyFile)
                                            {
                                                browseSecretFileTextBox.Text = browseEmptyFile;

                                                //UNCOMMENT GAK?
                                                /*browseBrowseButton.Enabled = false;
                                                browseSecretTextBox.ReadOnly = false;*/
                                            }
                                            else
                                            {
                                                if (File.Exists(browseSecretFileTextBox.Text))
                                                {
                                                    //UNCOMMENT GAK?
                                                    /*browseSecretTextBox.ReadOnly = true;
                                                    browseBrowseButton.Enabled = true;*/
                                                }
                                                else //path is not valid
                                                {
                                                    MessageBox.Show("Please enter a valid file path", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                    browseSecretFileTextBox.Text = browseEmptyFile;

                                                    /*browseSecretTextBox.ReadOnly = false;
                                                    browseBrowseButton.Enabled = false;*/
                                                }
                                            }
                                        };
                                        browseBrowseButton.Visible = true;
                                        RemoveClickEvent(browseBrowseButton);
                                        browseBrowseButton.Click += (o2, e2) =>
                                        {
                                            DialogResult result = browseOpenFileDialog.ShowDialog();
                                            if (result == DialogResult.OK)
                                            {
                                                string fileLocation = browseOpenFileDialog.FileName;
                                                browseSecretFileTextBox.Text = fileLocation;
                                                //UNCOMMENT?
                                                browseSecretTextBox.ReadOnly = true;
                                            }
                                            //else Debug.WriteLine("cancel");
                                        };
                                        browseGenerateButton.Visible = true;
                                        RemoveClickEvent(browseGenerateButton);
                                        browseGenerateButton.Click += (o2, e2) =>
                                        {
                                            byte[] secretBytes = null;
                                            if (browseSecretTextBox.Text != browseEmptyText)
                                            {
                                                //MessageBox.Show(browseSecretTextBox.Text);
                                                secretBytes = ASCIIEncoding.ASCII.GetBytes(browseSecretTextBox.Text);
                                                /*string test = ASCIIEncoding.ASCII.GetString(secretBytes);
                                                MessageBox.Show(test);*/
                                            }
                                            else if (browseSecretFileTextBox.Text != browseEmptyFile)
                                            {
                                                string SLocation = browseSecretFileTextBox.Text;
                                                try
                                                {
                                                    using (FileStream fs = new FileStream(SLocation, FileMode.Open, FileAccess.Read))
                                                    {
                                                        secretBytes = new byte[fs.Length];
                                                        int bytesLeft = (int)fs.Length;
                                                        int bytesRead = 0;
                                                        while (bytesLeft > 0)
                                                        {
                                                            int res = fs.Read(secretBytes, bytesRead, bytesLeft);
                                                            if (res == 0)
                                                                break;
                                                            bytesRead += res;
                                                            bytesLeft -= res;
                                                        }

                                                        //Debug.WriteLine(current_k + " & " + current_n);
                                                        /*using (FileStream fsWrite = new FileStream("test.png", FileMode.Create, FileAccess.Write))
                                                        {
                                                            fsWrite.Write(secretBytes, 0, secretBytes.Length);
                                                        }*/
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine(ex.ToString());
                                                }
                                            }
                                            //else hayoloh

                                            if (secretBytes == null)
                                            {
                                                Debug.WriteLine("HAYOLOH NULL");
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    DatabaseHandler.Connect();
                                                    List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                                                    byte k = (byte)(ulong)schemeInfos[3];
                                                    byte n = (byte)(ulong)schemeInfos[4];
                                                    DatabaseHandler.Close();
                                                    //Debug.WriteLine(k + " & " + n);
                                                    byte[][] byteMatrix = SecretSharing.GenerateByteShares(k, n, secretBytes);

                                                    DatabaseHandler.Connect();
                                                    List<string> players = DatabaseHandler.GetPlayers(scheme);
                                                    players.Sort();

                                                    byte idx = 0;
                                                    foreach (string player in players)
                                                    {
                                                        MQHandler.SendDirectMessage("Generate", "Share", scheme, username, player, byteMatrix[idx]);
                                                        idx++;
                                                    }

                                                    //TES
                                                    //byte[] testBytes = SecretSharing.ReconstructByteSecret(byteMatrix, current_k); //tested, pasti berhasil
                                                    MessageBox.Show("The shares have been sent to all players", "Shares Delivery Completd", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    MQHandler.Ack(DeliveryTag);
                                                    DatabaseHandler.Close();

                                                    browseGenerateButton.Visible = false;
                                                    ntfPanel.Controls.Remove(ntfButton);
                                                    ntfButton.Dispose();
                                                }
                                                catch (MySql.Data.MySqlClient.MySqlException ex)
                                                {
                                                    Debug.WriteLine(ex.Number + " : " + ex.Message);
                                                }
                                            }
                                        };
                                    };
                                    break;
                                }
                            case "Share":
                                {
                                    ntfButton.Text = "[" + scheme + "] You get a share from " + sender;
                                    ntfButton.Click += (o, e) =>
                                    {
                                        ntfActionStackPanel.SelectTab(0); //Action
                                        ntfConfLabel1.Text = sender + " has sent a share for you to save.";
                                        ntfConfLabel1.Visible = true;
                                        ntfConfLabel2.Text = "Click 'Save' to save the share to your local disk.";
                                        ntfConfLabel2.Visible = true;
                                        ntfConfButton1.Visible = false;
                                        ntfConfButton2.Text = "Save";
                                        ntfConfButton2.Visible = true;
                                        RemoveClickEvent(ntfConfButton2);
                                        ntfConfButton2.Click += (o1, e1) =>
                                        {
                                            //current_bytes = message;
                                            ntfSaveFileDialog.DefaultExt = "share";
                                            ntfSaveFileDialog.Filter = "Share document (*.share)|*.share";
                                            ntfSaveFileDialog.AddExtension = true;
                                            DialogResult dr = ntfSaveFileDialog.ShowDialog();
                                            if (dr == DialogResult.OK)
                                            {
                                                string filename = ntfSaveFileDialog.FileName;
                                                File.WriteAllBytes(filename, message);

                                                ntfConfLabel2.Text = "You have saved the share.";
                                                ntfConfButton2.Visible = false;

                                                MQHandler.Ack(DeliveryTag);
                                                ntfPanel.Controls.Remove(ntfButton);
                                                ntfButton.Dispose();
                                            }
                                        };
                                    };
                                    break;
                                }
                        }
                        break;
                    }
                case "Reconstruct":
                    {
                        switch(type)
                        {
                            case "Request":
                                {
                                    ntfButton.Text = "[" + scheme + "] " + sender + " requests secret reconstruction";
                                    ntfButton.Click += (o, e) =>
                                    {
                                        ntfActionStackPanel.SelectTab(0); //Action
                                        ntfConfLabel1.Text = sender + " has requested a secret reconstruction on scheme '" + scheme + "'";
                                        ntfConfLabel1.Visible = true;
                                        ntfConfLabel2.Text = "If you approved this request, upload and send your share. Otherwise, click 'Reject'";
                                        ntfConfLabel2.Visible = true;

                                        ntfConfButton1.Text = "Upload...";
                                        ntfConfButton1.Visible = true;
                                        RemoveClickEvent(ntfConfButton1);
                                        ntfConfButton1.Click += (o1, e1) =>
                                        {
                                            DialogResult result = ntfRecOpenFileDialog.ShowDialog();
                                            if (result == DialogResult.OK)
                                            {
                                                string shareFile = ntfRecOpenFileDialog.FileName;
                                                ntfConfLabel2.Text = "You have uploaded share file '" + Path.GetFileName(shareFile) + "'. Click 'Send' to send it to " + sender + ".";
                                                ntfConfButton1.Text = "Send";
                                                RemoveClickEvent(ntfConfButton1);
                                                ntfConfButton1.Click += (o11, e11) =>
                                                {
                                                    try
                                                    {
                                                        DatabaseHandler.Connect();
                                                        DatabaseHandler.IncrementConfirmations(scheme);
                                                        DatabaseHandler.Close();
                                                        byte[] shareBytes = null;
                                                        using (FileStream fs = new FileStream(shareFile, FileMode.Open, FileAccess.Read))
                                                        {
                                                            shareBytes = new byte[fs.Length];
                                                            int bytesLeft = (int)fs.Length;
                                                            int bytesRead = 0;
                                                            while (bytesLeft > 0)
                                                            {
                                                                int res = fs.Read(shareBytes, bytesRead, bytesLeft);
                                                                if (res == 0)
                                                                    break;
                                                                bytesRead += res;
                                                                bytesLeft -= res;
                                                            }
                                                        }
                                                        //byte[] shareBytes = ASCIIEncoding.ASCII.GetBytes(shareFile);
                                                        MQHandler.SendDirectMessage("Reconstruct", "Response", scheme, username, sender, shareBytes);

                                                        ntfConfLabel2.Text = "You have sent your share to " + sender + ".";
                                                        ntfConfButton1.Visible = false;
                                                        ntfConfButton2.Visible = false;
                                                        
                                                        MQHandler.Ack(DeliveryTag);
                                                        ntfPanel.Controls.Remove(ntfButton);
                                                        ntfButton.Dispose();

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(ex.ToString());
                                                    }
                                                };
                                            }
                                        };
                                        ntfConfButton2.Text = "Reject";
                                        ntfConfButton2.Visible = true;
                                        RemoveClickEvent(ntfConfButton2);
                                        ntfConfButton2.Click += (o2, e2) =>
                                        {
                                            try
                                            {
                                                DatabaseHandler.Connect();
                                                DatabaseHandler.IncrementConfirmations(scheme);
                                                DatabaseHandler.Close();
                                                MQHandler.SendDirectMessage("Reconstruct", "Response", scheme, username, sender, BitConverter.GetBytes(false));
                                                ntfConfLabel2.Text = "You have rejected " + sender + "'s reconstruction request.";
                                                ntfConfButton1.Visible = false;
                                                ntfConfButton2.Visible = false;

                                                MQHandler.Ack(DeliveryTag);
                                                ntfPanel.Controls.Remove(ntfButton);
                                                ntfButton.Dispose();
                                            }
                                            catch (MySql.Data.MySqlClient.MySqlException ex)
                                            {
                                                Debug.WriteLine(ex.Number + " " + ex.Message);
                                            }
                                        };
                                    };
                                    break;
                                }
                            case "Response":
                                {
                                    string bText = "", label1Text = "", label2Text = "";
                                    bool isApproved;
                                    if (message.Length > 1) //approve
                                    {
                                        isApproved = true;
                                        bText = "[" + scheme + "] " + sender + " approved your reconstruction request";
                                        label1Text = "Click 'Save' to save " + sender + "'s share to your disk.";
                                    }
                                    else //reject
                                    {
                                        isApproved = false;
                                        bText = "[" + scheme + "] " + sender + " rejected your reconstruction request";
                                        label1Text = sender + " has rejected your reconstruction request on scheme '" + scheme + "'";
                                    }
                                    ntfButton.Text = bText;
                                    ntfButton.Click += (o, e) =>
                                    {
                                        //MQHandler.Ack(DeliveryTag);
                                        ntfConfLabel1.Text = label1Text;
                                        ntfConfLabel1.Visible = true;
                                        try
                                        {
                                            DatabaseHandler.Connect();
                                            List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                                            ulong n = (ulong)schemeInfos[4];
                                            ulong num_of_confirmations = (ulong)schemeInfos[5];
                                            ntfConfLabel2.Text = "Number of confirmations so far : " + num_of_confirmations + "/" + n;
                                            ntfConfLabel2.Visible = true;
                                            ntfConfButton1.Visible = false;

                                            if (isApproved)
                                            {
                                                ntfConfButton2.Text = "Save";
                                                ntfConfButton2.Visible = true;
                                                RemoveClickEvent(ntfConfButton2);
                                                ntfConfButton2.Click += (o2, e2) =>
                                                {
                                                    ntfRecSaveFileDialog.DefaultExt = "share";
                                                    ntfRecSaveFileDialog.Filter = "Share document (*.share)|*.share";
                                                    ntfRecSaveFileDialog.AddExtension = true;
                                                    DialogResult dr = ntfRecSaveFileDialog.ShowDialog();
                                                    if (dr == DialogResult.OK)
                                                    {
                                                        string filename = ntfRecSaveFileDialog.FileName;
                                                        File.WriteAllBytes(filename, message);
                                                        MQHandler.Ack(DeliveryTag);
                                                        ntfConfLabel1.Text = "You have saved " + sender + "'s share. Please delete it after use.";
                                                        ntfConfButton2.Visible = false;
                                                        ntfPanel.Controls.Remove(ntfButton);
                                                        ntfButton.Dispose();
                                                    }
                                                };
                                            }
                                            else
                                            {
                                                ntfConfButton2.Visible = false;
                                                MQHandler.Ack(DeliveryTag);
                                                ntfPanel.Controls.Remove(ntfButton);
                                                ntfButton.Dispose();
                                            }
                                            DatabaseHandler.Close();
                                        }
                                        catch (MySql.Data.MySqlClient.MySqlException ex)
                                        {
                                            Debug.WriteLine(ex.Number + " : " + ex.Message);
                                            throw;
                                        }
                                    };
                                    break;
                                }
                        }
                        break;
                    }
                case "Update":
                    {
                        switch(type)
                        {
                            case "Request":
                                {
                                    ntfButton.Text = "[" + scheme + "] " + sender + " requests shares update";
                                    ntfButton.Click += (o, e) =>
                                    {
                                        ntfActionStackPanel.SelectTab(0); //Action
                                        ntfConfLabel1.Text = sender + " has requested shares update on scheme '" + scheme + "'";
                                        ntfConfLabel1.Visible = true;
                                        ntfConfLabel2.Text = "If you approved this request, click 'Confirm'. Otherwise, click 'Reject'";
                                        ntfConfLabel2.Visible = true;

                                        ntfConfButton1.Text = "Confirm";
                                        ntfConfButton1.Visible = true;
                                        RemoveClickEvent(ntfConfButton1);
                                        ntfConfButton1.Click += (o1, e1) =>
                                        {
                                            try
                                            {
                                                DatabaseHandler.Connect();
                                                DatabaseHandler.IncrementConfirmations(scheme);

                                                MQHandler.SendDirectMessage("Update", "Response", scheme, username, sender, BitConverter.GetBytes(true)); //here sender = destination!

                                                List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                                                string dealer = (string)schemeInfos[2];
                                                ulong n = (ulong)schemeInfos[4];
                                                ulong num_of_confirmations = (ulong)schemeInfos[5];
                                                //Send notifications if all players have been accepted
                                                if (num_of_confirmations == n)
                                                {
                                                    MQHandler.SendFanoutMessages("Update", "Notice", scheme, dealer, BitConverter.GetBytes(true));
                                                    //Handle dealer ga???
                                                    //MQHandler.SendDirectMessage("Update", "Dealer", scheme, "System", dealer, ASCIIEncoding.ASCII.GetBytes("")); //send special request to dealer
                                                }

                                                ntfConfLabel2.Text = "You approved this request.";
                                                ntfConfButton1.Visible = false;
                                                ntfConfButton2.Visible = false;
                                                MQHandler.Ack(DeliveryTag);
                                                ntfPanel.Controls.Remove(ntfButton);
                                                ntfButton.Dispose();
                                                DatabaseHandler.Close();
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine(ex.Message);
                                            };
                                        };

                                        ntfConfButton2.Text = "Reject";
                                        ntfConfButton2.Visible = true;
                                        RemoveClickEvent(ntfConfButton2);
                                        ntfConfButton2.Click += (o2, e2) =>
                                        {
                                            try
                                            {
                                                DatabaseHandler.Connect();
                                                DatabaseHandler.IncrementConfirmations(scheme);

                                                MQHandler.SendDirectMessage("Update", "Response", scheme, username, sender, BitConverter.GetBytes(false)); //here sender = destination!

                                                List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                                                string dealer = (string)schemeInfos[2];
                                                ulong n = (ulong)schemeInfos[4];
                                                ulong num_of_confirmations = (ulong)schemeInfos[5];
                                                //Send notifications if all players have been accepted
                                                MQHandler.SendFanoutMessages("Update", "Notice", scheme, "System", BitConverter.GetBytes(false));
                                                //send ke dealer juga ga?

                                                ntfConfLabel2.Text = "You rejected this request.";
                                                ntfConfButton1.Visible = false;
                                                ntfConfButton2.Visible = false;
                                                MQHandler.Ack(DeliveryTag);
                                                ntfPanel.Controls.Remove(ntfButton);
                                                ntfButton.Dispose();
                                                DatabaseHandler.Close();
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine(ex.Message);
                                            };
                                        };
                                    };
                                    break;
                                }
                            case "Response":
                                {
                                    bool isAccepted = BitConverter.ToBoolean(message, 0);
                                    string response = "";
                                    if (isAccepted)
                                    {
                                        response = "accepted";
                                    }
                                    else
                                    {
                                        response = "rejected"; ;
                                    }

                                    ntfButton.Text = "[" + scheme + "] " + sender + " " + response + " your shares update request";
                                    ntfButton.Click += (o, e) =>
                                    {
                                        ntfActionStackPanel.SelectTab(0); //Action
                                        ntfConfLabel1.Text = sender + " has " + response + " your request update the shares.";
                                        ntfConfLabel1.Visible = true;
                                        if (isAccepted)
                                        {
                                            try
                                            {
                                                DatabaseHandler.Connect();
                                                List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                                                ulong n = (ulong)schemeInfos[4];
                                                ulong num_of_confirmations = (ulong)schemeInfos[5];
                                                ntfConfLabel2.Text = "Number of confirmations so far : " + num_of_confirmations + "/" + n;
                                                ntfConfLabel2.Visible = true;
                                                DatabaseHandler.Close();
                                            }
                                            catch (MySql.Data.MySqlClient.MySqlException ex)
                                            {
                                                Debug.WriteLine("Something goes wrong on Database");
                                            }
                                        }
                                        else
                                        {
                                            ntfConfLabel2.Text = "This means the shares update operation is failed.";
                                            ntfConfLabel2.Visible = true;
                                        }
                                        ntfConfButton1.Visible = false;
                                        ntfConfButton2.Visible = false;
                                        MQHandler.Ack(DeliveryTag);
                                        ntfPanel.Controls.Remove(ntfButton);
                                        ntfButton.Dispose();
                                    };
                                    break;
                                }
                            case "Notice":
                                {
                                    bool isAccepted = BitConverter.ToBoolean(message, 0);
                                    string bText, label1Text, label2Text;
                                    if(isAccepted)
                                    {
                                        bText = "[" + scheme + "] " + " Update share operation advances to next step";
                                        label1Text = "Good news, all players have approved the update share request!";
                                        label2Text = "Now, please click the button below to generate and send subshares to all players.";
                                    }
                                    else
                                    {
                                        bText = "[" + scheme + "] " + " Update share operation is failed";
                                        label1Text = "Bad news, a player has rejected the update share request.";
                                        label2Text = "Thus, the update operation is failed and will not be processed further.";
                                    }

                                    ntfButton.Text = bText;
                                    ntfButton.Click += (o, e) =>
                                    {
                                        ntfActionStackPanel.SelectTab(0); //Action
                                        ntfConfLabel1.Text = label1Text;
                                        ntfConfLabel1.Visible = true;
                                        ntfConfLabel2.Text = label2Text;
                                        ntfConfLabel2.Visible = true;

                                        if(isAccepted)
                                        {
                                            ntfConfButton1.Visible = false;
                                            ntfConfButton2.Text = "Send Subshares";
                                            ntfConfButton2.Visible = true;

                                            RemoveClickEvent(ntfConfButton2);
                                            ntfConfButton2.Click += (o2, e2) =>
                                            {
                                                /*ntfUpdOpenFileDialog.DefaultExt = "share";
                                                ntfUpdOpenFileDialog.Filter = "Share document (*.share)|*.share";
                                                ntfUpdOpenFileDialog.AddExtension = true;
                                                DialogResult dr = ntfUpdOpenFileDialog.ShowDialog();
                                                if(dr == DialogResult.OK)
                                                {
                                                    //lanjut sini!!
                                                    string shareFile = ntfUpdOpenFileDialog.FileName;
                                                    
                                                    ntfConfLabel2.Text = "Subshares from share file '" + Path.GetFileName(shareFile) + "' have been generated. Click 'Send' to send them to other players.";
                                                    ntfConfButton2.Enabled = true;
                                                    ntfConfButton2.Click += (o2, e2) =>
                                                    {
                                                        //lanjut kk
                                                    };
                                                }*/

                                                try
                                                {
                                                    DatabaseHandler.Connect();
                                                    List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                                                    DatabaseHandler.Close();
                                                    byte k = (byte)(ulong)schemeInfos[3];
                                                    byte n = (byte)(ulong)schemeInfos[4];
                                                    DatabaseHandler.Connect();
                                                    List<string> players = DatabaseHandler.GetPlayers(scheme);
                                                    players.Sort();
                                                    DatabaseHandler.Close();

                                                    byte[] subshares = SecretSharing.GenerateByteSubshares(k, n);
                                                    Debug.Assert(players.Count == subshares.Length);
                                                    for(int i=0; i<players.Count; i++)
                                                    {
                                                        byte[] curByte = new byte[1];
                                                        curByte[0] = subshares[i];
                                                        MQHandler.SendDirectMessage("Update", "Subshare", scheme, username, players[i], curByte);
                                                    }
                                                    DatabaseHandler.Close();

                                                    MessageBox.Show("All subshares have been sent to all players", "Delivery Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    MQHandler.Ack(DeliveryTag);
                                                    ntfPanel.Controls.Remove(ntfButton);
                                                    ntfButton.Dispose();
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine(ex.Message);
                                                }
                                            };
                                        }
                                        else
                                        {
                                            ntfConfButton1.Visible = false;
                                            ntfConfButton2.Visible = false;
                                            MQHandler.Ack(DeliveryTag);
                                            ntfPanel.Controls.Remove(ntfButton);
                                            ntfButton.Dispose();
                                        }
                                    };
                                    break;
                                }
                            case "Subshare":
                                {
                                    ntfButton.Text = "[" + scheme + "] " + sender + " sent you a subshare";
                                    ntfButton.Click += (o, e) =>
                                    {
                                        ntfActionStackPanel.SelectTab(0); //Action
                                        ntfConfLabel1.Text = sender + " has sent you a subshare for share update on scheme '" + scheme + "'";
                                        ntfConfLabel1.Visible = true;
                                        //TODO : tampilin jumlah yg udah ngirim
                                        ntfConfLabel2.Text = "Click 'Save' to save the subshare to your disk";
                                        ntfConfLabel2.Visible = true;

                                        ntfConfButton1.Visible = false;
                                        ntfConfButton2.Text = "Save";
                                        ntfConfButton2.Visible = true;
                                        RemoveClickEvent(ntfConfButton2);
                                        ntfConfButton2.Click += (o2, e2) =>
                                        {
                                            ntfUpdSaveFileDialog.DefaultExt = "subshare";
                                            ntfUpdSaveFileDialog.Filter = "Subhare document (*.subshare)|*.subshare";
                                            ntfUpdSaveFileDialog.AddExtension = true;
                                            DialogResult dr = ntfUpdSaveFileDialog.ShowDialog();
                                            if(dr == DialogResult.OK)
                                            {
                                                string filename = ntfUpdSaveFileDialog.FileName;
                                                File.WriteAllBytes(filename, message);
                                            }
                                        };
                                    };
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        Debug.WriteLine("Something went wrong (Operation");
                        break;
                    }
            }

            //attach button to ntfPanel
            if (ntfPanel.InvokeRequired)
            {
                AddNtfButtonCallback anc = new AddNtfButtonCallback(AddNtfButton);
                this.Invoke(anc, new object[] { DeliveryTag, contents });
            }
            else
            {
                ntfPanel.Controls.Add(ntfButton);
            }
        }

        private void RemoveClickEvent(Button b)
        {
            FieldInfo f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(b);
            PropertyInfo pi = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
            list.RemoveHandler(obj, list[obj]);
        }

        /*      Other Methods       */
        /*internal static bool isFileOrDirectoryExists(string name)
        {
            return (Directory.Exists(name) || File.Exists(name));
        }*/

        //private byte[] FiletoBytes

        private void button1_Click(object sender, EventArgs e)
        {
            //DecrementNotifications();
            //notificationsButton.Text = "ADLADKJADKKKLAFFA";
            //MQHandler.SendDirectMessage("Generatehehe", "Response", "asdf", username, "arif", BitConverter.GetBytes(true)); //here sender = destination!
            MQHandler.SendFanoutMessages("Generate", "Dealer","asdf", username, BitConverter.GetBytes(true));
        }

        private void ntfSaveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            //string filename = ntfSaveFileDialog.FileName;
            //File.WriteAllBytes(filename, current_bytes);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string scheme = cekShareTextBox.Text;
            cekOpenFileDialog.Multiselect = true;
            DialogResult result = cekOpenFileDialog.ShowDialog();
            if(String.IsNullOrWhiteSpace(scheme))
            {
                MessageBox.Show("empty scheme");
            }
            else if (result == DialogResult.OK)
            {
                string[] shareFiles = cekOpenFileDialog.FileNames;
                cekSaveFileDialog.ShowDialog();
                DialogResult dr = ntfSaveFileDialog.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        DatabaseHandler.Connect();
                        List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                        byte k = (byte)(ulong)schemeInfos[3];
                        byte n = (byte)(ulong)schemeInfos[4];
                        SecretSharing.ReconstructFileSecret(shareFiles, k, cekSaveFileDialog.FileName);
                        //File.WriteAllBytes(filename, current_bytes);
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        Debug.WriteLine(ex.Number + " " + ex.Message);
                    }
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string scheme = cekShareTextBox.Text;
            if (String.IsNullOrWhiteSpace(scheme))
            {
                MessageBox.Show("empty scheme");
            }
            else
            {
                try
                {
                    DatabaseHandler.Connect();
                    DatabaseHandler.DeleteScheme(scheme);
                    DatabaseHandler.Close();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show("Something went wrong. Please try again.");
                }
                MQHandler.DeleteExchange(scheme);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
