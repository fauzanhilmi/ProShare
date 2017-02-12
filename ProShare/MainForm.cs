using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Security;

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
        private const string genPlayersCount2 = " participants";
        private string genEmptyPlayer;
        private const string genEmptyPlayer1 = "Enter an username (e.g., ";
        private const string genEmptyPlayer2 = ")";
        private const string genFirstStatus = "Sending share requests...";

        /*      RECONSTRUCT attributes      */
        private const string recEmptySchemeText = "You are not involved in any scheme";
        private const string recEmptyFile = "Select files";
        private string[] recShareFiles;

        /*      UPDATE attributes           */
        private const string updEmptySchemeText = "You are not involved in any scheme(s)";
        private const string updEmptyShare = "Select your share file";
        private const string updEmptySubshare = "Select subshare files";
        private string updShareFile;
        private string[] updSubshareFiles;

        /*      Notifications attributes         */
        private const int numOfNotifications = 0;
        private const string browseEmptyText = "Enter a text...";
        private const string browseEmptyFile = "Select files";
        //private byte[] current_bytes;
        //private byte current_k; //for Generate - Dealer
        //private byte current_n;
        //private string ntftText = "Notifications (";

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
            //TEST
            //byte[] clearBytes = Encoding.UTF8.GetBytes("­­­Ê");
            byte[] clearBytes = File.ReadAllBytes("subshare encrypted0.txt");
            Debug.WriteLine(clearBytes[0]);
            byte[] passBytes = Encoding.UTF8.GetBytes("6A6357C4506FD8EF0C98F29403388854828B1CE889F037894DBD5F37C5E57C51");
            /*byte[] encBytes = AESEncryptBytes(clearBytes, passBytes);
            Debug.WriteLine("enc = " + Encoding.UTF8.GetString(encBytes));
            File.WriteAllBytes("enc.txt", encBytes);*/
            byte[] decBytes = AESDecryptBytes(clearBytes, passBytes);
            Debug.WriteLine("dec = "+Encoding.UTF8.GetString(decBytes));
            File.WriteAllBytes("dec.txt", decBytes);


            //BENERAN TEST
            /*string step1 = "cuma test heh";
            string tkey = "B9EF9CA48768B2D2D2AD1F4487A7414G";

            Console.WriteLine("step 1 = " + step1);
            byte[] step2 = AESEncryptBytes(Encoding.UTF8.GetBytes(step1), Encoding.UTF8.GetBytes(tkey));
            Console.WriteLine("step 2 = " + Encoding.UTF8.GetString(step2));
            byte[] step3 = AESDecryptBytes(step2, Encoding.UTF8.GetBytes(tkey));
            Console.WriteLine("step 3 = " + Encoding.UTF8.GetString(step3));*/

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

            /*              UPDATE initializations                  */
            updGenerateSchemesComboBox.Text = updEmptySchemeText;
            updGenerateShareTextBox.Text = updEmptyShare;
            updGenerateSubsharesTextBox.Text = updEmptySubshare;

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
                            List<string> schemes1 = DatabaseHandler.GetSchemesByDealer(username);
                            DatabaseHandler.Close();
                            DatabaseHandler.Connect();
                            List<string> schemes2 = DatabaseHandler.GetSchemesByPlayer(username);
                            schemes = schemes1.Concat(schemes2).ToList();
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
                            List<string> schemes1 = DatabaseHandler.GetSchemesByDealer(username);
                            DatabaseHandler.Close();
                            DatabaseHandler.Connect();
                            List<string> schemes2 = DatabaseHandler.GetSchemesByPlayer(username);
                            schemes = schemes1.Concat(schemes2).ToList();
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
                            updGenerateShareBrowseButton.Enabled = false;
                            updGenerateSubharesBrowseButton.Enabled = false;
                            updGenerateSchemesComboBox.Text = updEmptySchemeText;
                            updGenerateButton.Enabled = false;
                        }
                        else
                        {
                            BindingSource bs = new BindingSource();
                            bs.DataSource = schemes;
                            updSendSchemesComboBox.DataSource = bs;
                            updSendButton.Enabled = true;
                            updGenerateShareBrowseButton.Enabled = true;
                            updGenerateSubharesBrowseButton.Enabled = true;
                            BindingSource bs2 = new BindingSource();
                            bs2.DataSource = schemes;
                            updGenerateSchemesComboBox.DataSource = bs;
                            updGenerateButton.Enabled = false;
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
                MessageBox.Show("You typed empty username", "Cannot Add Participant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                int index = genPlayersListBox.FindStringExact(genPlayerTextBox.Text);
                if(index != -1) //participant is already added
                {
                    MessageBox.Show("You have already added this username", "Participant Addition Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (genPlayerTextBox.Text == username)
                {
                    MessageBox.Show("You cannot become a participant", "Participant Addition Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                        string randPass = Membership.GeneratePassword(10, 10);
                        string key = HashSHA256(randPass);

                        //TEST ENCRYPTION
                        //string key = HashMD5(randPass);
                        //END OF TEST
                        int addRes = DatabaseHandler.AddScheme(schemeName, username, k, n, key); //try to add scheme
                        
                        if (addRes == 1) //success
                        {
                            //genStatusLabel.Text = genFirstStatus;
                            genStatusLabel.Visible = true;
                            genDontCloseLabel.Visible = true;

                            DatabaseHandler.AddPlayers(schemeName, players); //add players to db

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
                        MessageBox.Show("A participant's name(s) does not exist. Please check it again.", "Share Requests Delivery Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    MQHandler.SendFanoutMessages("Reconstruct", "Request", scheme, username, UTF8Encoding.UTF8.GetBytes(""));
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
                Console.WriteLine(recSharesTextBox.Text);
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
                            string key = (string)schemeInfos[6];
                            if(recShareFiles.Length < k)
                            {
                                MessageBox.Show("Number of shares are less than number of participants!", "Reconstruction Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                //TEST
                                SecretSharing.ReconstructFileSecret(recShareFiles, k, recSaveFileDialog.FileName);
                                //TEST ENCRYPTION SHARE RECONSTRUCTION 
                                //SecretSharing.ReconstructFileSecret(recShareFiles, k, recSaveFileDialog.FileName, Encoding.UTF8.GetBytes(key));
                                /*byte[][] sharesBytes = new byte[k][];
                                for(int i=0; i<sharesBytes.Length; i++)
                                {
                                    byte[] encryptedShare = File.ReadAllBytes(recShareFiles[i]);
                                    //File.WriteAllBytes("before "+i + ".txt", encryptedShare);
                                    sharesBytes[i] = AESDecryptBytes(encryptedShare, Encoding.UTF8.GetBytes(key));
                                    //File.WriteAllBytes("after "+i + ".txt", sharesBytes[i]);
                                }*
                                byte[] secretBytes = SecretSharing.ReconstructByteSecret(sharesBytes, k);
                                File.WriteAllBytes(recSaveFileDialog.FileName, secretBytes);*/


                                //File.WriteAllBytes(filename, current_bytes);
                                MessageBox.Show("Secret reconstruction process is completed", "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
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

                    MQHandler.SendFanoutMessages("Update", "Request", scheme, username, UTF8Encoding.UTF8.GetBytes(""));
                    MessageBox.Show("Update requests delivery is completed", "Delivery Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Unexpected " + ex.Number + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void updGenerateShareBrowseButton_Click(object sender, EventArgs e)
        {
            updGenerateShareOpenFileDialog.DefaultExt = "share";
            updGenerateShareOpenFileDialog.Filter = "Share document (*.share)|*.share";
            updGenerateShareOpenFileDialog.AddExtension = true;
            DialogResult dr = updGenerateShareOpenFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                updShareFile = updGenerateShareOpenFileDialog.FileName;
                updGenerateShareTextBox.Text = Path.GetFileName(updShareFile);
                if (updGenerateSubsharesTextBox.Text != updEmptySubshare)
                {
                    updGenerateButton.Enabled = true;
                }                
            }
        }

        private void updGenerateSusbharesBrowseButton_Click(object sender, EventArgs e)
        {
            updGenerateSubsharesOpenFileDialog.DefaultExt = "subshare";
            updGenerateSubsharesOpenFileDialog.Filter = "Subhare document (*.subshare)|*.subshare";
            updGenerateSubsharesOpenFileDialog.AddExtension = true;
            updGenerateSubsharesOpenFileDialog.Multiselect = true;
            DialogResult dr = updGenerateSubsharesOpenFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string[] filenames = updGenerateSubsharesOpenFileDialog.FileNames;
                updSubshareFiles = filenames;
                string oneline = "";
                foreach (string file in filenames)
                {
                    string current = "\"" + Path.GetFileName(file) + "\"";
                    oneline += current;
                }
                updGenerateSubsharesTextBox.Text = oneline;
                if (updGenerateShareTextBox.Text != updEmptyShare)
                {
                    updGenerateButton.Enabled = true;
                }
            }
        }

        private void updGenerateButton_Click(object sender, EventArgs e)
        {
            string scheme = updGenerateSchemesComboBox.Text;
            if (scheme == updEmptySchemeText)
            {
                //do nothing
            }
            else if (updGenerateShareTextBox.Text != updEmptyShare && updGenerateSubsharesTextBox.Text != updEmptySubshare)
            {
                updSaveFileDialog.DefaultExt = "share";
                updSaveFileDialog.Filter = "Share document (*.share)|*.share";
                updSaveFileDialog.AddExtension = true;

                DialogResult dr = updSaveFileDialog.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        DatabaseHandler.Connect();
                        List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                        byte k = (byte)(ulong)schemeInfos[3];
                        byte n = (byte)(ulong)schemeInfos[4];
                        string key = (string)schemeInfos[6];

                        //ganti method read byte??
                        byte[] oldShareBytes = File.ReadAllBytes(updShareFile);
                        byte[] subshareBytes = new byte[updSubshareFiles.Length];
                        for (int i = 0; i < updSubshareFiles.Length; i++)
                        {
                            byte[] curBytes = File.ReadAllBytes(updSubshareFiles[i]);
                            /*File.WriteAllBytes("UPDATE susbhare no " + i + ".subshare", curBytes);*/
                            subshareBytes[i] = curBytes[0];

                            //TEST ENCRYPTION SHArE UPDATE
                            /*byte[] decryptedCurBytes = AESDecryptBytes(curBytes, Encoding.UTF8.GetBytes(key));
                            File.WriteAllBytes("UPDATE decrypted no "+i+".subshare", decryptedCurBytes);
                            Debug.Assert(decryptedCurBytes.Length == 1);
                            subshareBytes[i] = decryptedCurBytes[0];*/
                        }

                        byte[] newShareBytes = SecretSharing.GenerateNewShareBytes(oldShareBytes, subshareBytes);
                        File.WriteAllBytes("UPDATE real share budi.share", newShareBytes);

                        File.WriteAllBytes(updSaveFileDialog.FileName, newShareBytes);

                        //TEST ENCRYPTIOON
                        /*byte[] encryptedShareBytes = AESEncryptBytes(newShareBytes, Encoding.UTF8.GetBytes(key));
                        File.WriteAllBytes(updSaveFileDialog.FileName, encryptedShareBytes);*/

                        MessageBox.Show("New share has been saved. Please delete your old share.", "Operation Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DatabaseHandler.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
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
            string operation = Encoding.UTF8.GetString((byte[])contents["Operation"]);
            string type = Encoding.UTF8.GetString((byte[])contents["Type"]);
            string scheme = Encoding.UTF8.GetString((byte[])contents["Scheme"]);
            //current_scheme = scheme;
            string sender = Encoding.UTF8.GetString((byte[])contents["Sender"]);
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
                                        ntfConfLabel1.Text = sender + " wants you to join his secret sharing scheme '" + scheme + "' as a participant.";
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
                                                    MQHandler.SendDirectMessage("Generate", "Dealer", scheme, "System", dealer, UTF8Encoding.UTF8.GetBytes("")); //send special request to dealer
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
                                        ntfConfLabel1.Text = sender + " has " + response + " your request to join scheme '" + scheme + "' as participant";
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
                                        label1Text = "Good news, all participants have accepted the share requests!";
                                        label2Text = "Now just wait for the dealer (" + sender + ") to distributes the shares.";
                                    }
                                    else
                                    {
                                        bText = "[" + scheme + "] " + " Scheme fails to advance to share distribution";
                                        label1Text = "Bad news, one or more participant(s) have rejected the share request.";
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
                                                //secretBytes = UTF8Encoding.UTF8.GetBytes(browseSecretTextBox.Text);
                                                //TEST
                                                secretBytes = Encoding.UTF8.GetBytes(browseSecretTextBox.Text);
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
                                                    string key = (string)schemeInfos[6];
                                                    DatabaseHandler.Close();

                                                    Console.WriteLine(key);                                                    
                                                    byte[][] byteMatrix = SecretSharing.GenerateByteShares(k, n, secretBytes);
                                                    //TEST ENCRYPTION SHARE GENERATION
                                                    //byte[][] byteMatrix = SecretSharing.GenerateEncryptedByteShares(k, n, secretBytes, Encoding.UTF8.GetBytes(key));

                                                    DatabaseHandler.Connect();
                                                    List<string> players = DatabaseHandler.GetPlayers(scheme);
                                                    players.Sort();

                                                    byte idx = 0;
                                                    foreach (string player in players)
                                                    {
                                                        MQHandler.SendDirectMessage("Generate", "Share", scheme, username, player, byteMatrix[idx]);
                                                        idx++;
                                                    }

                                                    MessageBox.Show("The shares have been sent to all participants", "Shares Delivery Completd", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                                            ntfRecOpenFileDialog.DefaultExt = "share";
                                            ntfRecOpenFileDialog.Filter = "Share document (*.share)|*.share";
                                            ntfRecOpenFileDialog.AddExtension = true;
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
                                                        //byte[] shareBytes = UTF8Encoding.UTF8.GetBytes(shareFile);
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
                                        ntfActionStackPanel.SelectTab(0); //Action
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
                                        ntfConfLabel2.Text = "If you approved this request, click 'Approve'. Otherwise, click 'Reject'";
                                        ntfConfLabel2.Visible = true;

                                        ntfConfButton1.Text = "Approve";
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
                                                //Send notifications if all participants have been accepted
                                                if (num_of_confirmations == n)
                                                {
                                                    MQHandler.SendFanoutMessages("Update", "Notice", scheme, dealer, BitConverter.GetBytes(true));
                                                    MQHandler.SendDirectMessage("Update", "Dealer", scheme, sender, dealer, BitConverter.GetBytes(true));
                                                    //MQHandler.SendDirectMessage("Update", "Dealer", scheme, "System", dealer, UTF8Encoding.UTF8.GetBytes("")); //send special request to dealer
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
                                                //Send notifications if all particiapnts have rejected
                                                MQHandler.SendFanoutMessages("Update", "Notice", scheme, "System", BitConverter.GetBytes(false));
                                                MQHandler.SendDirectMessage("Update", "Dealer", scheme, sender, dealer, BitConverter.GetBytes(false));

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
                            case "Dealer":
                                {
                                    bool isAccepted = BitConverter.ToBoolean(message, 0);
                                    ntfButton.Text = "[" + scheme + "] " + sender + " has " + (isAccepted ? "successfully" : "failedly") + " requested shares update";
                                    ntfButton.Click += (o, e) =>
                                    {
                                        ntfActionStackPanel.SelectTab(0);
                                        ntfConfLabel1.Text = sender + " has requested shares update requests to all participants";
                                        ntfConfLabel1.Visible = true;

                                        if (isAccepted)
                                        {
                                            ntfConfLabel2.Text = "All participants accepted the requests. They are in the process of updating now.";
                                        }
                                        else
                                        {
                                            ntfConfLabel2.Text = "Some participant(s) rejected the request(s). Thus, the update operation is failed.";
                                        }
                                        ntfConfLabel2.Visible = true;
                                        ntfConfButton1.Visible = false;
                                        ntfConfButton2.Visible = false;

                                        MQHandler.Ack(DeliveryTag);
                                        ntfPanel.Controls.Remove(ntfButton);
                                        ntfButton.Dispose();
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
                                        bText = "[" + scheme + "] " + "Update share operation advances to next step";
                                        label1Text = "Good news, all participants have approved the update share request!";
                                        label2Text = "Now, please click the 'Send' button to generate and send subshares to all participants.";
                                    }
                                    else
                                    {
                                        bText = "[" + scheme + "] " + "Update share operation is failed";
                                        label1Text = "Bad news, a participant has rejected the update share request.";
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
                                            ntfConfButton2.Text = "Send";
                                            ntfConfButton2.Visible = true;

                                            RemoveClickEvent(ntfConfButton2);
                                            ntfConfButton2.Click += (o2, e2) =>
                                            {
                                                try
                                                {
                                                    DatabaseHandler.Connect();
                                                    List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                                                    DatabaseHandler.Close();
                                                    byte k = (byte)(ulong)schemeInfos[3];
                                                    byte n = (byte)(ulong)schemeInfos[4];
                                                    string key = (string)schemeInfos[6];
                                                    DatabaseHandler.Connect();
                                                    List<string> players = DatabaseHandler.GetPlayers(scheme);
                                                    players.Sort();
                                                    DatabaseHandler.Close();

                                                    byte[] subshares = SecretSharing.GenerateByteSubshares(k, n);
                                                    //ENCRYPTION SUBSHARE GENERATION
                                                    //byte[] subshares = SecretSharing.GenerateEncryptedByteSubshares(k, n, Encoding.UTF8.GetBytes(key));
                                                    //TEST ENCRYPTIOON V2
                                                    //byte[][] subshares = SecretSharing.GenerateEncryptedByteSubshares(k, n, Encoding.UTF8.GetBytes(key));

                                                    Debug.Assert(players.Count == subshares.Length);
                                                    for(int i=0; i<players.Count; i++)
                                                    {
                                                        byte[] curByte = new byte[1];
                                                        curByte[0] = subshares[i];
                                                        MQHandler.SendDirectMessage("Update", "Subshare", scheme, username, players[i], curByte);
                                                    }

                                                    //TEST ENCRYPTIOON V2
                                                    /*for (int i=0; i<players.Count; i++)
                                                    {
                                                        byte[] curBytes = subshares[i];
                                                        File.WriteAllBytes("subshare on send" + i + ".txt", curBytes);
                                                        MQHandler.SendDirectMessage("Update", "Subshare", scheme, username, players[i], curBytes);
                                                    }*/

                                                    DatabaseHandler.Close();
                                                     //MessageBox.Show("All subshares have been sent to all participants", "Delivery Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                     ntfConfLabel2.Text = "All subshares have been sent to all participants";
                                                    ntfConfButton2.Visible = false;

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

                                                ntfConfLabel2.Text = "Subshare has been saved";
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

        /*private void button2_Click(object sender, EventArgs e)
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
        }*/

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private string HashSHA256(string inputText)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputText);
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private static string HashMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private byte[] AESEncryptBytes(byte[] clearBytes, byte[] passBytes)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes("12345678");
            byte[] encryptedBytes = null;

            // create a key from the password and salt, use 32K iterations – see note
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 256);

            // create an AES object
            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        private byte[] AESDecryptBytes(byte[] cryptBytes, byte[] passBytes)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes("12345678");
            byte[] clearBytes = null;

            // create a key from the password and salt, use 32K iterations
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 256);

            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cryptBytes, 0, cryptBytes.Length);
                        cs.Close();
                    }
                    clearBytes = ms.ToArray();
                }
            }
            return clearBytes;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        /*private static byte[] AESEncryptBytes(byte[] inputBytes, byte[] key)
        {
            string IV = "1234567890123456";
            AesCryptoServiceProvider dataencrypt = new AesCryptoServiceProvider();
            //Block size : Gets or sets the block size, in bits, of the cryptographic operation.  
            dataencrypt.BlockSize = 128;
            //KeySize: Gets or sets the size, in bits, of the secret key  
            dataencrypt.KeySize = 128;
            //Key: Gets or sets the symmetric key that is used for encryption and decryption.  
            dataencrypt.Key = key;
            //IV : Gets or sets the initialization vector (IV) for the symmetric algorithm  
            dataencrypt.IV = System.Text.Encoding.UTF8.GetBytes(IV);
            //Padding: Gets or sets the padding mode used in the symmetric algorithm  
            dataencrypt.Padding = PaddingMode.PKCS7;
            //Mode: Gets or sets the mode for operation of the symmetric algorithm  
            dataencrypt.Mode = CipherMode.CBC;
            //Creates a symmetric AES encryptor object using the current key and initialization vector (IV).  
            ICryptoTransform crypto1 = dataencrypt.CreateEncryptor(dataencrypt.Key, dataencrypt.IV);
            //TransformFinalBlock is a special function for transforming the last block or a partial block in the stream.   
            //It returns a new array that contains the remaining transformed bytes. A new array is returned, because the amount of   
            //information returned at the end might be larger than a single block when padding is added.  
            byte[] encrypteddata = crypto1.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            crypto1.Dispose();
            //return the encrypted data  
            return encrypteddata;
        }

        private static byte[] AESDecryptBytes(byte[] inputBytes, byte[] key)
        {
            string IV = "1234567890123456";
            AesCryptoServiceProvider keydecrypt = new AesCryptoServiceProvider();
            keydecrypt.BlockSize = 128;
            keydecrypt.KeySize = 128;
            keydecrypt.Key = key;
            keydecrypt.IV = System.Text.Encoding.UTF8.GetBytes(IV);
            keydecrypt.Padding = PaddingMode.PKCS7;
            keydecrypt.Mode = CipherMode.CBC;
            ICryptoTransform crypto1 = keydecrypt.CreateDecryptor(keydecrypt.Key, keydecrypt.IV);

            byte[] returnbytearray = crypto1.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            crypto1.Dispose();
            return returnbytearray;
        }*/
    }
}
