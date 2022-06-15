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

            //tb_User.Text = "Admin";
            //tb_Pass.Text = "123";

        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            if (TCPClientV2.Instance.Login(tb_User.Text, tb_Pass.Text))
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

        
    }
}
