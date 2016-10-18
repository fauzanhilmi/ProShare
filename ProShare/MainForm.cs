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
        /*      MainForm attributes         */
        private string defaultNotificationsText = "Notifications ("; //minus the right side!
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
            //TEST
            username = "fauzan";
            try
            {
                DatabaseHandler.Connect();
                DatabaseHandler.IncrementConfirmations("lmao");
                DatabaseHandler.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Something went wrong. Please try again.");
            }
            /*MQHandler.Connect();
            MQHandler.Ack(123);
            MQHandler.Close();*/
            //TEST

            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = ColorTranslator.FromHtml("#00B0F0");

            menuGroupBox.Text = "Hello, " + username + "!";
            notificationsButton.Text = defaultNotificationsText + "0)";

            this.FormClosing += MainForm_FormClosing;

            MQHandler.Connect();
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
            ntfConfLabel1.Visible = false;
            ntfConfLabel2.Visible = false;
            ntfConfButton1.Visible = false;
            ntfConfButton2.Visible = false;
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

            //changing number of notifications on notificationsButton
            ulong numOfNotifications = (ulong)notificationsButton.Text[15] - '0'; //change index if the text is changed!
            Debug.WriteLine(notificationsButton.Text + " : " + numOfNotifications);
            numOfNotifications++;
            string newNotificationsText = defaultNotificationsText + numOfNotifications + ")";
            SetText(notificationsButton, newNotificationsText);

            //adding notification button
            AddNtfButton(DeliveryTag, contents);

            //TEST
            /*Debug.WriteLine(">" + DeliveryTag);
            foreach(var item in contents)
            {
                Debug.WriteLine(item.Key + " : " + Encoding.ASCII.GetString((byte[]) item.Value) + " (" + item.Value.ToString() + ")");
            }
            Debug.WriteLine("");*/
        }

        private void SetText(Control obj, string text)
        {
            //Debug.WriteLine(text);
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
            //TEST
            /*foreach (var item in contents)
            {
                Debug.WriteLine(item.Key + " : " + Encoding.ASCII.GetString((byte[])item.Value) + " (" + item.Value.ToString() + ")");
            }
            Debug.WriteLine("");*/

            Button ntfButton = new Button();
            ntfButton.Tag = DeliveryTag;
            ntfButton.Name = "ntfButton" + DeliveryTag;
            ntfButton.BackColor = SystemColors.GradientInactiveCaption;
            ntfButton.FlatStyle = FlatStyle.Flat;
            ntfButton.FlatAppearance.BorderColor = SystemColors.ScrollBar;
            ntfButton.Dock = DockStyle.Top;
            ntfButton.TextAlign = ContentAlignment.TopLeft;

            //setting the button text
            string type = Encoding.ASCII.GetString((byte[])contents["Type"]);
            string operation = Encoding.ASCII.GetString((byte[])contents["Operation"]);
            string scheme = Encoding.ASCII.GetString((byte[])contents["Scheme"]);
            string sender = Encoding.ASCII.GetString((byte[])contents["Sender"]);
            if (type == "REQUEST")
            {
                switch (operation)
                {
                    case "Generate":
                        {
                            ntfButton.Text = "(Share Request) " + sender + " invites you to join scheme '" + scheme + "'";
                            ntfButton.Click += (o, e) =>
                            {
                                ntfActionStackPanel.SelectTab(0); //Action
                                ntfConfLabel1.Text = sender + " wants you to join his secret sharing scheme '" + scheme + "' as a player.";
                                ntfConfLabel1.Visible = true;
                                ntfConfLabel2.Text = "Click 'Accept' to accept this request. Otherwise, click 'Reject'.";
                                ntfConfLabel2.Visible = true;

                                //ntfConfButton1.Tag = false; //button hasn't been clicked
                                ntfConfButton1.Text = "Accept";
                                ntfConfButton1.Visible = true;
                                ntfConfButton1.Click += (o1, e1) =>
                                {
                                    MQHandler.Ack(DeliveryTag);
                                    MQHandler.SendDirectMessage("RESPONSE", "Generate", scheme, username, sender, BitConverter.GetBytes(true)); //here sender = destination!
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
                                };
                                //ntfConfButton2.Tag = false; //button hasn't been clicked
                                ntfConfButton2.Text = "Reject";
                                ntfConfButton2.Visible = true;
                                ntfConfButton2.Click += (o2, e2) =>
                                {
                                    MQHandler.Ack(DeliveryTag);
                                    MQHandler.SendDirectMessage("RESPONSE", "Generate", scheme, username, sender, BitConverter.GetBytes(false)); //here sender = destination!

                                    //Delete exchange waktu semua dapet announcement aja
                                    //DELETE DB JUGA SAMA (biar aman)
                                    /*try
                                    {
                                        DatabaseHandler.Connect();
                                        DatabaseHandler.DeleteScheme(scheme);
                                        DatabaseHandler.Close();
                                    }
                                    catch (MySql.Data.MySqlClient.MySqlException ex)
                                    {
                                        Debug.WriteLine(ex.Number + " : " + ex.Message);
                                        MessageBox.Show("Something went wrong. Please try again");
                                    }*/
                                    ntfConfLabel2.Text = "You rejected this request.";
                                    ntfConfButton1.Visible = false;
                                    ntfConfButton2.Visible = false;
                                };
                            };
                            break;
                        }
                    case "Reconstruct":
                        {
                            //TODO
                            break;
                        }
                    case "Default":
                        {
                            //TODO
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine("Something went wrong (Operation)");
                            break;
                        }
                }
            }
            else if (type == "RESPONSE")
            {
                switch(operation)
                {
                    case "Generate":
                        {

                            break;
                        }
                    case "Reconstruct":
                        {
                            //TODO
                            break;
                        }
                    case "Default":
                        {
                            //TODO
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine("Something went wrong (Operation)");
                            break;
                        }
                }
            }
            else
            {
                Debug.WriteLine("Something went wrong (Type)");
            }
            //newButton.Text = text;

            //newButton.Click += NewButton_Click;

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

        /*private void NewButton_Click(object button, EventArgs e)
        {
            //MessageBox.Show(((Button)sender).Tag.ToString());
            ulong DeliveryTag = (ulong)((Button)button).Tag;
            string type = Encoding.ASCII.GetString((byte[])ntfDictionary[DeliveryTag]["Type"]);
            string operation = Encoding.ASCII.GetString((byte[])ntfDictionary[DeliveryTag]["Operation"]);
            string scheme = Encoding.ASCII.GetString((byte[])ntfDictionary[DeliveryTag]["Scheme"]);
            string sender = Encoding.ASCII.GetString((byte[])ntfDictionary[DeliveryTag]["Sender"]);
            if (type == "REQUEST")
            {
                switch (operation)
                {
                    case "Generate":
                        {
                            ntfActionStackPanel.SelectTab(0); //Action
                            ntfConfLabel1.Text = sender + " wants you to join his secret sharing scheme '" + scheme + "' as a player.";
                            ntfConfLabel1.Visible = true;
                            ntfConfLabel2.Text = "Click 'Accept' to accept this request. Otherwise, click 'Reject'.";
                            ntfConfLabel2.Visible = true;

                            ntfConfButton1.Tag = DeliveryTag;
                            ntfConfButton1.Text = "Accept";
                            ntfConfButton1.Visible = true;
                            ntfConfButton2.Tag = DeliveryTag;
                            ntfConfButton2.Text = "Reject";
                            ntfConfButton2.Visible = true;
                            break;
                        }
                    case "Reconstruct":
                        {
                            //TODO
                            break;
                        }
                    case "Default":
                        {
                            //TODO
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine("Something went wrong (Operation)");
                            break;
                        }
                }
            }
            else if (type == "RESPONSE")
            {
                //TODO
            }
            else
            {
                Debug.WriteLine("Something went wrong (Type)");
            }
        }

        private void ntfConfButton1_Click(object button, EventArgs e)
        {
            ulong DeliveryTag = (ulong)((Button)button).Tag;
            string type = Encoding.ASCII.GetString((byte[])ntfDictionary[DeliveryTag]["Type"]);
            string operation = Encoding.ASCII.GetString((byte[])ntfDictionary[DeliveryTag]["Operation"]);
            string scheme = Encoding.ASCII.GetString((byte[])ntfDictionary[DeliveryTag]["Scheme"]);
            string sender = Encoding.ASCII.GetString((byte[])ntfDictionary[DeliveryTag]["Sender"]);

        }*/

        /*      Other Methods       */
        internal static bool isFileOrDirectoryExists(string name)
        {
            return (Directory.Exists(name) || File.Exists(name));
        }
    }
}
