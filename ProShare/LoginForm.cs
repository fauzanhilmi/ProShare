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

namespace ProShare
{
    public partial class LoginForm : Form
    {
        private string emptyUsernameText = "Username";
        private string emptyPasswordText = "Password";
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
    }
}
