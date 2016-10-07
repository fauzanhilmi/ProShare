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

            usernameTextBox.Text = emptyUsernameText;
            usernameTextBox.GotFocus += UsernameTextBox_GotFocus;
            usernameTextBox.LostFocus += UsernameTextBox_LostFocus;

            passwordTextBox.Text = emptyPasswordText;
            passwordTextBox.GotFocus += PasswordTextBox_GotFocus;
            passwordTextBox.LostFocus += PasswordTextBox_LostFocus;

            

            //passwordMaskedTextBox.
            /*try
            {
                AccountDatabase.Connect();
                string uid = "tranquillo";
                string pass = "barnetta";
                Debug.WriteLine(AccountDatabase.Add(uid, pass));
                //Debug.WriteLine(AccountDatabase.isExist(uid, pass));
                AccountDatabase.CloseConnection();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if(ex.Number == 1042) //Connection Error 
                {
                    Debug.WriteLine("Connection Error (" + ex.Number + ") : " + ex.Message);
                }
            }*/            
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
                    AccountDatabase.Connect();
                    string username = usernameTextBox.Text;
                    string password = passwordTextBox.Text;
                    password = HashSHA256(password);
                    try
                    {
                        int result = AccountDatabase.isExist(username, password);
                        if(result == 1)
                        {
                            MessageBox.Show("SUCCESS");
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
                    AccountDatabase.CloseConnection();
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
                MessageBox.Show("Username and Password field cannot be empty", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //Handle proper username / password format?
            else
            {
                try
                {
                    AccountDatabase.Connect();
                    string username = usernameTextBox.Text;
                    string password = passwordTextBox.Text;
                    password = HashSHA256(password);
                    try
                    {
                        int result = AccountDatabase.Add(username, password);
                        if(result ==  1)
                        {
                            MessageBox.Show("SUCCESS");
                            //proceed!
                        }
                        else if(result == 0)
                        {
                            MessageBox.Show("This computer is already registered. Please login with appropiate account.", "Register Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        //else catch exception
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Unexpected " + ex.Number + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    AccountDatabase.CloseConnection();
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
