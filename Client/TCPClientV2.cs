using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public class TCPClientV2
    {
        #region make singleton
        private static TCPClientV2 instance;
        public static TCPClientV2 Instance
        {
            get
            {
                if (instance == null) instance = new TCPClientV2(); return instance;
            }
            private set { instance = value; }
        }

        private TCPClientV2() { }
        #endregion

        TcpClient tcpCLient;
        Socket socketClient;
        const int SERVERPORT = 8080;
        const int SERVERPORT_SendData = 8079;
        Thread clientThread;
        bool stopTcpCient = true;
        public byte[] dataFromServer;

        public string username;
        public string password;

        public bool Login(string _username, string _password)
        {
            username = _username;
            password = _password;

            try
            {
                stopTcpCient = false;

                tcpCLient = new TcpClient();
                IPAddress serverIP = IPAddress.Parse("127.0.0.1");
                IPEndPoint iPEndPoint = new IPEndPoint(serverIP, SERVERPORT);

                tcpCLient.Connect(iPEndPoint);// Mở kết nối tới server 
                socketClient = tcpCLient.Client;// Lấy cổng kết nối để dùng 

                //Gửi username và password tới server
                string stringUsernameAndPassword = username + " " + password + " ";
                byte[] byteUsernameAndPassword = Encoding.ASCII.GetBytes(stringUsernameAndPassword);

                socketClient.Send(byteUsernameAndPassword);


                //kiểm tra xem phản hồi từ server là đăng nhập thành công hay không 
                byte[] _data = new byte[12];
                socketClient.Receive(_data);
                string isLoged = Encoding.UTF8.GetString(_data);


                //Nếu dăng nhập thành công thì mở MainWindow
                if (isLoged == "Successfully")
                {
                    
                    stopTcpCient = false;

                    //clientThread = new Thread(ClientReceive);
                    //clientThread.Start();

                    return true;
                }
                else
                {
                    //tcpCLient.Close();
                    //socketClient.Close();

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        void ClientReceive()
        {
            //dataFromServer = new byte[1024 * 5000];
            socketClient = tcpCLient.Client;

            byte[] data = new byte[1024 * 5000];

            while (/*tcpCLient.Connected &&*/ !stopTcpCient && clientThread.IsAlive == true)
            {
                Application.DoEvents();
                try
                {
                    socketClient.Receive(data);
                    dataFromServer = data;

                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }

            //socketClient.Close();
            //tcpCLient.Close();
            //clientThread.Abort();
        }

        public byte[] SendMessage(string command)
        {
            byte[] bytecommand = Encoding.ASCII.GetBytes(command);

            byte[] dataReturn = new byte[1024 * 5000];

            socketClient.Send(bytecommand);

            socketClient.Receive(dataReturn);

            return dataReturn;
        }

        TcpClient newtcpCLient;
        IPAddress newserverIP = IPAddress.Parse("127.0.0.1");
        public byte[] GetDataFromCommand(string command)
        {
            byte[] dataGet = new byte[1024 * 5000];
            
            IPEndPoint newiPEndPoint = new IPEndPoint(newserverIP, SERVERPORT_SendData);
            newtcpCLient = new TcpClient();
            newtcpCLient.Connect(newiPEndPoint);// Mở kết nối tới server 
            Socket newclientSocket = newtcpCLient.Client;

            byte[] dataSend = Encoding.ASCII.GetBytes(command);
            newclientSocket.Send(dataSend);//Gui

            newclientSocket.Receive(dataGet);//Nhan

            newclientSocket.Close();
            newtcpCLient.Close();


            return dataGet;
        }

        public void TCPCLientStop()
        {
            byte[] dataGet = new byte[1024 * 5000];
            //SERVERPORT++;
            IPEndPoint newiPEndPoint = new IPEndPoint(newserverIP, SERVERPORT + 1);
            newtcpCLient = new TcpClient();
            newtcpCLient.Connect(newiPEndPoint);// Mở kết nối tới server 
            Socket newclientSocket = newtcpCLient.Client;

            byte[] dataSend = Encoding.ASCII.GetBytes("EXIT");
            newclientSocket.Send(dataSend);//Gui

            newclientSocket.Receive(dataGet);//Nhan

            newclientSocket.Close();
            newtcpCLient.Close();

            stopTcpCient = true;
            tcpCLient.Close();
            clientThread.Abort();
        }
    }
}
