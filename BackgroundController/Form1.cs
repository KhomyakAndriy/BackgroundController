using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackgroundController
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            ShowInTaskbar = false;
            Visible = false;
            InitializeComponent();
            notifyIcon.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            form.ShowInTaskbar = false;
            form.Location = new Point(-10000, -10000);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Icon = null;
            notifyIcon.Dispose();
            Application.Exit();
        }
    }
}
