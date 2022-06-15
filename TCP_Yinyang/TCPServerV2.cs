using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Yinyang
{
    public class TCPServerV2
    {
        #region make singleton
        private static TCPServerV2 instance;
        public static TCPServerV2 Instance
        {
            get
            {
                if (instance == null) instance = new TCPServerV2(); return instance;
            }
            private set { instance = value; }
        }

        private TCPServerV2() { }
        #endregion

        Thread serverThread;
        TcpListener tcpListener;
        bool isServerOpen = false;
        int SERVERPORT = 8080;
        Dictionary<string, TcpClient> tcpClientDictionary = new Dictionary<string, TcpClient>();// lưu những tcp đc tạo ra mỗi khi có 1 client đăng nhập

        Dictionary<string, Thread> threadDictionary = new Dictionary<string, Thread>();// lưu những thread đc tạo ra mỗi khi có 1 client đăng nhập

        public void TCPServerStart()
        {
            isServerOpen = true;
            serverThread = new Thread(new ThreadStart(Listen));
            serverThread.Start();
        }

        private void Listen()
        {
            try
            {
                tcpListener = new TcpListener(new System.Net.IPEndPoint(IPAddress.Any, SERVERPORT));
                tcpListener.Start();

                while (isServerOpen)
                {

                    TcpClient _tcpClient = tcpListener.AcceptTcpClient();

                    Socket _socketClient = _tcpClient.Client;

                    byte[] data = new byte[1024 * 500];
                    _socketClient.Receive(data);
                    string _login = Encoding.ASCII.GetString(data);

                    string[] usernameAndpassword = _login.Split(' ', '\0');

                    if (usernameAndpassword[0] == "" && usernameAndpassword[1] == "")
                    {
                        string LoggedInSuccessfully = "Successfully";
                        byte[] Loged = Encoding.UTF8.GetBytes(LoggedInSuccessfully);
                        _socketClient.Send(Loged); // thong bao toi client dang nhap thanh cong


                        //Tao 1 thread moi để phục vụ user này 
                        Thread clientThread = new Thread(() => CommandFromClient(usernameAndpassword[0], _tcpClient));
                        //tcpClientDictionary.Add(usernameAndpassword[0], _tcpClient);
                        //clientThread.Name = usernameAndpassword[0];
                        //threadDictionary.Add(usernameAndpassword[0], clientThread);
                        clientThread.Start();
                    }
                    else
                    {
                        string LoginFailed = "LoginFailed";
                        byte[] LogFail = Encoding.UTF8.GetBytes(LoginFailed);

                        _socketClient.Send(LogFail);// thong bao toi client dang nhap khong thanh cong
                    }

                    //_socketClient.Close();
                    //_tcpClient.Close();

                    //_socketClient.Dispose();
                    //_socketClient = null;
                    //_tcpClient.Dispose();
                    //_tcpClient = null;
                    //tcpListener.Stop();
                }

                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void CommandFromClient(string username, TcpClient tcpClient)
        {

            bool clientConnecting = true;

            TcpClient tcpClient1 = tcpClient;
            Socket newSocket = tcpClient1.Client;

            //TcpListener tcpListener1 = new TcpListener(new System.Net.IPEndPoint(IPAddress.Any, SERVERPORT_SendData)); //SERVERPORT_SendData--;
            //tcpListener1.Start();

            while (isServerOpen && clientConnecting)
            {

                try
                {

                    byte[] data = new byte[1024 * 5000];
                    newSocket.Receive(data);

                    string message = Encoding.UTF8.GetString(data);
                    string[] msg = message.Split('\0', ' ');


                    switch (msg[0])
                    {
                        case "GetMyUsername":
                            byte[] dataHello = new byte[1024 * 5000];
                            dataHello = Encoding.UTF8.GetBytes("Hello " + msg[1]);

                            newSocket.Send(dataHello);

                            //newSocket.Close();
                            //tcpClient1.Close();

                            break;

                        case "GetDataTable":
                            byte[] dataTable = new byte[1024 * 5000];
                            dataTable = FormatData.Instance.SerializeData(FormatData.Instance.getdata());

                            newSocket.Send(dataTable);
                            
                            //newSocket.Close();
                            //tcpClient1.Close();
                            break;

                        case "MainWindow":

                            break;
                        case "EXIT":
                            newSocket.Close();
                            tcpClient1.Close();

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

        public void TCPServerStop()
        {
            foreach (var item in tcpClientDictionary)
            {
                tcpClientDictionary.Remove(item.Key);
                Thread threadBeKill = threadDictionary[item.Key];
                threadDictionary.Remove(item.Key);
                threadBeKill.Abort();
            }
            isServerOpen = false;
            tcpListener.Stop();
            
            serverThread.Abort();

        }
    }
}
