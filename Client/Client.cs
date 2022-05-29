using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();

            tb_User.Text = "Admin";
            tb_Pass.Text = "123";

            this.FormClosing += Client_FormClosing;
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            if (TCPClient.Instance.Login(tb_User.Text, tb_Pass.Text))
            {
                this.Hide();
                MainWindow w = new MainWindow();
                w.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Sai tk hoac mat khau");
            }
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Do you really want to exit?", "Dialog Title", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    TCPClient.Instance.TCPCLientStop();

                    //Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
