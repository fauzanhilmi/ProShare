﻿using System;
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
        private string genEmptyName = "Enter a name";
        private string genEmptyText = "Enter a text here...";
        private string genEmptyFile = "Select a file";
        private string genPlayersCount1 = "Add ";
        private string genPlayersCount2 = " players";
        private string genEmptyPlayer;
        private string genEmptyPlayer1 = "Enter an username (e.g., ";
        private string genEmptyPlayer2 = ")";
        private string genFirstStatus = "Sending share requests...";


        /*      Notifications attributes         */
        private int numOfNotifications = 0;
        private string browseEmptyText = "Enter a text...";
        private string browseEmptyFile = "Select a file";
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

            //string sch = "BODO";
            //try
            //{
            //    DatabaseHandler.Connect();
            //    DatabaseHandler.DeleteScheme(sch);
            //    DatabaseHandler.Close();
            //}
            //catch (MySql.Data.MySqlClient.MySqlException ex)
            //{
            //    MessageBox.Show("Something went wrong. Please try again.");
            //}
            //MQHandler.DeleteExchange(sch);
            //TEST

            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = ColorTranslator.FromHtml("#00B0F0");

            menuGroupBox.Text = "Hello, " + username + "!";
            //notificationsButton.Text = defaultNotificationsText + "0)";
            //notificationsButton.Text = ntfLeftText + numOfNotifications + ")";

            this.FormClosing += MainForm_FormClosing;

            MQHandler.GetMessage(username, ProcessNotification); //Listening to incoming notifications
            ntfDictionary = new Dictionary<ulong, IDictionary<string, object>>();

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

            //Browse
            //IMPLEMENT!!!
            //browseSecretTextBox.GotFocus += BrowseSecretTextBox_GotFocus;
            //browseSecretTextBox.LostFocus += BrowseSecretTextBox_LostFocus;
            //browseSecretFileTextBox.GotFocus += BrowseSecretFileTextBox_GotFocus;
            //browseSecretFileTextBox.LostFocus += BrowseSecretFileTextBox_LostFocus;
            //browseBrowseButton.Click += BrowseBrowseButton_Click;
            //browseGenerateButton.Click += BrowseGenerateButton_Click;
        }

        //PINDAHIN KE NOTIFICATION
        /*private void BrowseSecretTextBox_GotFocus(object sender, EventArgs e)
        {
            if (browseSecretTextBox.Text == browseEmptyText)
            {
                browseSecretTextBox.Text = "";
            }
        }

        //coba 2 method ini!
        private void BrowseSecretTextBox_LostFocus(object sender, EventArgs e)
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
        }

        private void BrowseSecretFileTextBox_GotFocus(object sender, EventArgs e)
        {
            if (browseSecretFileTextBox.Text == browseEmptyFile)
            {
                browseSecretFileTextBox.Text = "";
            }
        }

        private void BrowseSecretFileTextBox_LostFocus(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(browseSecretFileTextBox.Text) || browseSecretFileTextBox.Text == browseEmptyFile)
            {
                browseSecretFileTextBox.Text = browseEmptyFile;

                //UNCOMMENT GAK?
                //browseBrowseButton.Enabled = false;
                //browseSecretTextBox.ReadOnly = false;
            }
            else
            {
                if(File.Exists(browseSecretFileTextBox.Text))
                {
                    //UNCOMMENT GAK?
                    //browseSecretTextBox.ReadOnly = true;
                    //browseBrowseButton.Enabled = true;
                }
                else //path is not valid
                {
                    MessageBox.Show("Please enter a valid file path", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    browseSecretFileTextBox.Text = browseEmptyFile;

                    //browseSecretTextBox.ReadOnly = false;
                    //browseBrowseButton.Enabled = false;
                }
            }
        }

        private void BrowseBrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = browseOpenFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileLocation = browseOpenFileDialog.FileName;
                browseSecretFileTextBox.Text = fileLocation;
                //UNCOMMENT?
                browseSecretTextBox.ReadOnly = true;
            }
        }
        
        private void BrowseGenerateButton_Click(object sender, EventArgs e)
        {
            byte[] secretBytes = null;
            if (browseSecretTextBox.Text != browseEmptyText)
            {
                //MessageBox.Show(browseSecretTextBox.Text);
                secretBytes = ASCIIEncoding.ASCII.GetBytes(browseSecretTextBox.Text);
                //string test = ASCIIEncoding.ASCII.GetString(secretBytes);
                //MessageBox.Show(test);
            }
            else if(browseSecretFileTextBox.Text != browseEmptyFile)
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
                        //using (FileStream fsWrite = new FileStream("test.png", FileMode.Create, FileAccess.Write))
                        //{
                        //    fsWrite.Write(secretBytes, 0, secretBytes.Length);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            //else hayoloh

            if(secretBytes == null)
            {
                Debug.WriteLine("HAYOLOH NULL");
            }
            else
            {
                try
                {
                    DatabaseHandler.Connect();
                    List<object> schemeInfos = DatabaseHandler.GetScheme(current_scheme);
                    byte k = (byte)(ulong)schemeInfos[3];
                    byte n = (byte)(ulong)schemeInfos[4];
                    DatabaseHandler.Close();
                    //Debug.WriteLine(k + " & " + n);
                    byte[][] byteMatrix = SecretSharing.GenerateByteShares(k, n, secretBytes);

                    DatabaseHandler.Connect();
                    List<string> players = DatabaseHandler.GetPlayers(current_scheme);

                    byte idx = 0;
                    foreach(string player in players)
                    {
                        MQHandler.SendDirectMessage("Generate", "Share", current_scheme, username, player, byteMatrix[idx]);
                        idx++;
                    }
                    //disini
                    //on track to send bytematrix to players

                    //TES
                    //byte[] testBytes = SecretSharing.ReconstructByteSecret(byteMatrix, current_k); //tested, pasti berhasil
                    DatabaseHandler.Close();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    Debug.WriteLine(ex.Number + " : " + ex.Message);
                }
            }

        }*/

        /*private void SaveBytestoFile(byte[] bytes, string FLocation)
        {
            try
            {
                using (FileStream fsWrite = new FileStream(FLocation, FileMode.Create, FileAccess.Write))
                {
                    fsWrite.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }*/
        //END OF NANTI PINDAHIN

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
                            //TEST


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
                                        ntfConfButton1.Click += (o1, e1) => //If scheme is already deleted, it does nothing
                                        {
                                            MQHandler.Ack(DeliveryTag);
                                            MQHandler.SendDirectMessage("Generate", "Response", scheme, username, sender, BitConverter.GetBytes(true)); //here sender = destination!
                                            try
                                            {
                                                DatabaseHandler.Connect();
                                                DatabaseHandler.IncrementConfirmations(scheme);
                                                DatabaseHandler.Close();
                                            }
                                            catch (MySql.Data.MySqlClient.MySqlException ex)
                                            {
                                                Debug.WriteLine(ex.Number + " : " + ex.Message);
                                                MessageBox.Show("Something went wrong. Please try again");
                                            }
                                            ntfConfLabel2.Text = "You accepted this request.";
                                            ntfConfButton1.Visible = false;
                                            ntfConfButton2.Visible = false;
                                            ntfPanel.Controls.Remove(ntfButton);
                                            ntfButton.Dispose();
                                            //DecrementNotifications(); //Why it isn't working :(
                                        };

                                        ntfConfButton2.Text = "Reject";
                                        ntfConfButton2.Visible = true;
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

                                                //Send notifications if all players have been accepted
                                                if(num_of_confirmations == n)
                                                {
                                                    MQHandler.SendFanoutMessages("Generate", "Notice", scheme, username, BitConverter.GetBytes(true));
                                                    MQHandler.SendDirectMessage("Generate", "Dealer", scheme, "System", username, ASCIIEncoding.ASCII.GetBytes("")); //send special request to dealer
                                                }
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
                                        bText = "[" + scheme + "] " + " Scheme failed to advances to share distribution";
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
                                        browseSecretTextBox.GotFocus += (o2, e2) =>
                                        {
                                            if (browseSecretTextBox.Text == browseEmptyText)
                                            {
                                                browseSecretTextBox.Text = "";
                                            }
                                        };
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
                                        browseSecretFileTextBox.GotFocus += (o2, e2) =>
                                        {
                                            if (browseSecretFileTextBox.Text == browseEmptyFile)
                                            {
                                                browseSecretFileTextBox.Text = "";
                                            }
                                        };
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
                                        ntfConfButton2.Click += (o1, e1) =>
                                        {
                                            //current_bytes = message;
                                            ntfSaveFileDialog.DefaultExt = "share";
                                            ntfSaveFileDialog.Filter = "Share document (*.share)|*.share";
                                            ntfSaveFileDialog.AddExtension = true;
                                            ntfSaveFileDialog.FileOk += (o3, e3) =>
                                            {
                                                string filename = ntfSaveFileDialog.FileName;
                                                File.WriteAllBytes(filename, message);
                                            };
                                            ntfSaveFileDialog.ShowDialog();

                                            ntfConfLabel2.Text = "You have saved the share.";
                                            ntfConfButton2.Visible = false;

                                            MQHandler.Ack(DeliveryTag);
                                            ntfPanel.Controls.Remove(ntfButton);
                                            ntfButton.Dispose();
                                        };
                                    };
                                    break;
                                }
                        }
                        break;
                    }
                case "Reconstruct":
                    {
                        break;
                    }
                case "Update":
                    {
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
            if (result == DialogResult.OK)
            {
                string[] shareFiles = cekOpenFileDialog.FileNames;
                cekSaveFileDialog.ShowDialog();
                string filename = cekSaveFileDialog.FileName;
                try
                {
                    DatabaseHandler.Connect();
                    List<object> schemeInfos = DatabaseHandler.GetScheme(scheme);
                    byte k = (byte)(ulong)schemeInfos[3];
                    byte n = (byte)(ulong)schemeInfos[4];
                    Debug.WriteLine(k + " & " + n);
                    SecretSharing.ReconstructFileSecret(shareFiles, k, filename);
                    //File.WriteAllBytes(filename, current_bytes);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    Debug.WriteLine(ex.Number + " " + ex.Message);
                }
            }
        }
    }
}
