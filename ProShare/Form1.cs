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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AccountDatabase.Connect();
            string uid = "tranquillo";
            string pass = "barnetta";
            Debug.WriteLine(AccountDatabase.Add(uid, pass));
            //Debug.WriteLine(AccountDatabase.isExist(uid, pass));
            AccountDatabase.CloseConnection();
        }
    }
}
