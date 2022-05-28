using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            Load();

        }

        void Load()
        {
            Application.DoEvents();
            NewWay();
        }

        public object DeserializeData(byte[] theByteArray)
        {
            try
            {
                MemoryStream ms = new MemoryStream(theByteArray);
                BinaryFormatter bf1 = new BinaryFormatter();
                ms.Position = 0;
                return bf1.Deserialize(ms);
            }
            catch
            {
                return null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewWay();
        }

        void NewWay()
        {

            byte[] data = ClientThread.Instance.GetDataFromCommand("GetDataTable");
            try
            {
                DataTable dt = (DataTable)DeserializeData(data);
                dataGridView1.DataSource = dt;
            }
            catch
            {

            }

            data = ClientThread.Instance.GetDataFromCommand("GetMyUsername");
            string messageFromServer = Encoding.UTF8.GetString(data);

            //thử ép kiểu data nhận về là string
            try
            {
                string _messageFromServer = Encoding.UTF8.GetString(data);
                tb_Mess.Text = _messageFromServer;

            }
            catch
            {

            }
        }

    }
}
