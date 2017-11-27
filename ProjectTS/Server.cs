using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ProjectTS
{
    public class Server
    {
        const string DEFAULT_SERVER = "localhost";
        const int DEFAULT_PORT = 804;

        bool isConnected = false;

        //Server socket stuff
        Socket serverSocket;
        Socket clientSocket;

        public void Startup()
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(DEFAULT_SERVER);
            IPAddress serverAddr = hostInfo.AddressList[1];

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(serverAddr, DEFAULT_PORT));

            Console.WriteLine("Server started at:" + serverSocket.LocalEndPoint.ToString());

            int backlog = 0; //how many pending connections the queue will hold

            try
            {
                serverSocket.Listen(backlog);
                Console.WriteLine("Server listening");
                clientSocket = serverSocket.Accept();
                isConnected = true;
                Console.WriteLine("Client connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to listen" + ex.ToString());
            }
            this.Run();
        }

        public void Run()
        {
            while (isConnected)
            {
                ReceiveData();
                SendData();
            }
        }

        public void ReceiveData()
        {
            byte[] buffer = new byte[256];
            try
            {
                var bytesrecd = clientSocket.Receive(buffer);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Server received packet");
        }

        public void SendData()
        {
            Packet pack = new Packet();
            try
            {
                clientSocket.Send(pack.GetBytes());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                isConnected = false;
            }
        }
    }
}
