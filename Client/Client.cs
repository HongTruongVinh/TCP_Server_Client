﻿using System;
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
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            if (ClientThread.Instance.Logined())
            {
                this.Hide();
                MainWindow w = new MainWindow();
                w.ShowDialog();
                this.ShowDialog();
            }
        }

    }
}
