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
        }

        Thread serverThread;
        TcpListener tcpListener;
        bool isServerOpen = false;
        const int SERVERPORT = 8080;
        Dictionary<string, TcpClient> tcpClientDictionary = new Dictionary<string, TcpClient>();// lưu những tcp đc tạo ra mỗi khi có 1 client đăng nhập

        Dictionary<string, Thread> threadDictionary = new Dictionary<string, Thread>();// lưu những thread đc tạo ra mỗi khi có 1 client đăng nhập

        private void btn_Open_Click(object sender, EventArgs e)
        {

            if(isServerOpen == false)
            {
                btn_Open.Enabled = false;
                isServerOpen = true;
                btn_Open.Text = "Listening ...";

                TCPServer.Instance.TCPServerStart();
                //serverThread = new Thread(new ThreadStart(Listen));
                //serverThread.Start();
            }
            

        }

        void Listen()
        {
            try
            {

                tcpListener = new TcpListener(new System.Net.IPEndPoint(IPAddress.Any, SERVERPORT));
                tcpListener.Start();

                while (isServerOpen)
                {
                    TcpClient _tcpClient = tcpListener.AcceptTcpClient();

                    Socket _socketClient = _tcpClient.Client;

                    byte[] data = new byte[1024*500];
                    _socketClient.Receive(data);
                    string _login = Encoding.ASCII.GetString(data);

                    string[] usernameAndpassword = _login.Split(' ', '\0');
                    
                    if (usernameAndpassword[0] == "Admin" && usernameAndpassword[1] == "123")
                    {
                        string LoggedInSuccessfully = "Successfully";
                        byte[] Loged = Encoding.UTF8.GetBytes(LoggedInSuccessfully);
                        _socketClient.Send(Loged); // thong bao toi client dang nhap thanh cong


                        //Tao 1 thread moi để phục vụ user này 
                        Thread clientThread = new Thread(() => ClientReceive(usernameAndpassword[0], _tcpClient, _socketClient));
                        tcpClientDictionary.Add(usernameAndpassword[0], _tcpClient);
                        clientThread.Name = usernameAndpassword[0];
                        threadDictionary.Add(usernameAndpassword[0], clientThread);
                        clientThread.Start();
                    }
                    else
                    {
                        string LoginFailed = "LoginFailed";
                        byte[] LogFail = Encoding.UTF8.GetBytes(LoginFailed);

                        _socketClient.Send(LogFail);// thong bao toi client dang nhap khong thanh cong
                    }

                }


            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void ClientReceive(string username, TcpClient tcpClient, Socket socketClient)
        {

            bool clientConnecting = true;

            TcpListener tcpListener1 = new TcpListener(new System.Net.IPEndPoint(IPAddress.Any, SERVERPORT + 1));
            tcpListener1.Start();

            while (isServerOpen && clientConnecting)
            {

                try
                {
                    TcpClient tcpClient1 = tcpListener1.AcceptTcpClient();
                    Socket newSocket = tcpClient1.Client;

                    byte[] data = new byte[1024 * 5000];
                    newSocket.Receive(data);

                    string message = Encoding.UTF8.GetString(data);
                    string[] msg = message.Split('\0');


                    switch (msg[0])
                    {
                        case "GetMyUsername":
                            byte[] dataHello = new byte[1024 * 5000];
                            dataHello = Encoding.UTF8.GetBytes("Hello " + username);


                            newSocket.Send(dataHello);

                            newSocket.Close();
                            tcpClient1.Close();

                            break;

                        case "GetDataTable":

                            newSocket.Send(SerializeData(getdata()));

                            newSocket.Close();
                            tcpClient1.Close();
                            break;

                        case "MainWindow":

                            break;
                        case "Exit":
                            tcpClientDictionary.Remove(username);
                            Thread threadBeKill = threadDictionary[username];
                            threadDictionary.Remove(username);
                            threadBeKill.Abort();
                            clientConnecting = false;
                            break;
                    }
                }
                catch
                {

                }
            }
        }

        public byte[] SerializeData(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }

        /*Ở đây tôi tạo 1 bảng dữ liệu thử.
 Bạn có thể kết nối csdl và load dữ liệu lên*/
        private DataTable getdata()
        {
            DataTable dt = new DataTable();
            DataRow dr;

            dt.Columns.Add(new DataColumn("IntegerValue", typeof(Int32)));
            dt.Columns.Add(new DataColumn("StringValue", typeof(string)));
            dt.Columns.Add(new DataColumn("DateTimeValue", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("BooleanValue", typeof(bool)));

            for (int i = 1; i <= 1000; i++)
            {
                dr = dt.NewRow();
                dr[0] = i;
                dr[1] = "Item " + i.ToString();
                dr[2] = DateTime.Now;
                dr[3] = (i % 2 != 0) ? true : false;

                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
