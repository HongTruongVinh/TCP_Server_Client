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
            this.FormClosing += Client_FormClosing;
            Application.DoEvents();
            NewWayV2();
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
            //NewWay();

            NewWayV2();
        }

        void NewWayV2()
        {
            byte[] data = TCPClientV2.Instance.SendMessage("GetDataTable");

            DataTable dt = (DataTable)DeserializeData(data);

            dataGridView1.DataSource = dt;



            //DataTable dt2 = (DataTable)DeserializeData(data);

            data = TCPClientV2.Instance.SendMessage("GetMyUsername VinhHongTruong");

            string stringFromServer = Encoding.UTF8.GetString(data);

            tb_Mess.Text = stringFromServer;
        }

        void NewWay()
        {

            byte[] data1 = TCPClient.Instance.GetDataFromCommand("GetDataTable");
            DataTable dt;
            try
            {
                dt = (DataTable)DeserializeData(data1);
                dataGridView1.DataSource = dt;
            }
            catch
            {

            }

            byte[] data2 = TCPClient.Instance.GetDataFromCommand("GetMyUsername");
            string messageFromServer = Encoding.UTF8.GetString(data2);

            //thử ép kiểu data nhận về là string
            try
            {
                string _messageFromServer = Encoding.UTF8.GetString(data2);
                tb_Mess.Text = _messageFromServer;

            }
            catch
            {

            }


            dt = (DataTable)DeserializeData(data1);
            CopyDataTableToListView(dt, listView1);
        }

        private void CopyDataTableToListView(DataTable data, ListView lv)
        {
            lv.BeginUpdate();

            // if column count is different (typically not yet populated)
            if (lv.Columns.Count != data.Columns.Count)
            {
                lv.Columns.Clear();

                // prepare columns
                foreach (DataColumn column in data.Columns)
                {
                    lv.Columns.Add(column.ColumnName);
                }
            }

            // clear rows
            lv.Items.Clear();

            // load rows
            foreach (DataRow row in data.Rows)
            {
                ListViewItem item = new ListViewItem(row[0].ToString());
                for (int i = 1; i < data.Columns.Count; i++)
                {
                    item.SubItems.Add(row[i].ToString());
                }
                lv.Items.Add(item);
            }

            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lv.EndUpdate();
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
