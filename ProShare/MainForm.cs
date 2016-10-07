using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProShare
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = ColorTranslator.FromHtml("#00B0F0");
        }

        private void operationsButton_Click(object sender, EventArgs e)
        {
            stackPanel.SelectTab("Operations");
        }

        private void requestsButton_Click(object sender, EventArgs e)
        {
            stackPanel.SelectTab("Requests");
        }
    }
}
