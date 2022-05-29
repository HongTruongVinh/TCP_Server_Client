using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_Yinyang
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();

            Load();
        }
        void Load()
        {
            this.FormClosing += Server_FormClosing;
        }

        bool isServerOpen = false;

        private void btn_Open_Click(object sender, EventArgs e)
        {

            if(isServerOpen == false)
            {
                btn_Open.Enabled = false;
                isServerOpen = true;
                btn_Open.Text = "Listening ...";

                TCPServer.Instance.TCPServerStart();
            }
            

        }
        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Do you really want to exit?", "Dialog Title", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    TCPServer.Instance.TCPServerStop();

                    Environment.Exit(0);
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
