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
using System.Security.Cryptography;

namespace ProShare
{
    public partial class StartForm : Form
    {
        private string emptyUsernameText = "Username";
        private string emptyPasswordText = "Password";
        public StartForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = ColorTranslator.FromHtml("#00B0F0");

            Image logo = Image.FromFile("../../../images/logo.png");
            logoPictureBox.Image = logo;

            waitLabel.Visible = false;

            usernameTextBox.Text = emptyUsernameText;
            usernameTextBox.KeyDown += UsernameTextBox_KeyDown;
            usernameTextBox.GotFocus += UsernameTextBox_GotFocus;
            usernameTextBox.LostFocus += UsernameTextBox_LostFocus;

            passwordTextBox.Text = emptyPasswordText;
            passwordTextBox.KeyDown += PasswordTextBox_KeyDown;
            passwordTextBox.GotFocus += PasswordTextBox_GotFocus;
            passwordTextBox.LostFocus += PasswordTextBox_LostFocus;    
        }

        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                loginButton.PerformClick();
            }
        }

        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                loginButton.PerformClick();
            }
        }

        private void UsernameTextBox_GotFocus(object sender, EventArgs e)
        {
            if(usernameTextBox.Text == emptyUsernameText)
            {
                usernameTextBox.Text = "";
            }
        }

        private void UsernameTextBox_LostFocus(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                usernameTextBox.Text = emptyUsernameText;
            }
        }

        private void PasswordTextBox_GotFocus(object sender, EventArgs e)
        {
            if (passwordTextBox.Text == emptyPasswordText)
            {
                passwordTextBox.Text = "";
            }
            passwordTextBox.PasswordChar = '*';
        }

        private void PasswordTextBox_LostFocus(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                passwordTextBox.PasswordChar = (char)0;
                passwordTextBox.Text = emptyPasswordText;
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(usernameTextBox.Text) || usernameTextBox.Text == emptyUsernameText || String.IsNullOrWhiteSpace(passwordTextBox.Text) || passwordTextBox.Text == emptyPasswordText)
            {
                MessageBox.Show("Username and Password field cannot be empty", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Handle proper username / password format?
            else
            {
                try
                {
                    DatabaseHandler.Connect();
                    string username = usernameTextBox.Text;
                    string password = passwordTextBox.Text;
                    password = HashSHA256(password);
                    try
                    {
                        //TEST
                        //int result = DatabaseHandler.DoesAccountExist(username, password);
                        //TEST
                        int result = DatabaseHandler.DoesAccountExistDUMMY(username, password);
                        if (result == 1) //success
                        {
                            waitLabel.Visible = true;
                            //Form transition
                            this.Hide();
                            MainForm mf = new MainForm(username);
                            this.Owner = mf;
                            mf.Show();
                        }
                        else if(result == 0)
                        {
                            MessageBox.Show("Username or password doesn't match", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if(result == -1)
                        {
                            MessageBox.Show("This computer hasn't registered yet", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        }

        private void registetButton_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(usernameTextBox.Text) || usernameTextBox.Text == emptyUsernameText || String.IsNullOrWhiteSpace(passwordTextBox.Text) || passwordTextBox.Text == emptyPasswordText)
            {
                MessageBox.Show("Username and Password field cannot be empty", "Register Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Handle proper username / password format?
            else
            {
                try
                {
                    DatabaseHandler.Connect();
                    string username = usernameTextBox.Text;
                    string password = passwordTextBox.Text;
                    password = HashSHA256(password);
                    try
                    {
                        int result = DatabaseHandler.AddAccount(username, password);
                        if(result ==  1) //success
                        {
                            waitLabel.Visible = true;

                            //Create user queue
                            MQHandler.Connect();
                            MQHandler.CreateQueue(username);
                            MQHandler.Close();

                            //Form transition
                            MessageBox.Show("Welcome to Proshare, " + username + "!", "Registration Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            MainForm mf = new MainForm(username);
                            this.Owner = mf;
                            mf.Show();
                        }
                        else if(result == 0)
                        {
                            MessageBox.Show("This computer is already registered. Please login with appropiate account.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
}
