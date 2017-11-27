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

        //Server socket stuff
        Socket serverSocket;
        Socket clientSocket;

        SocketInformation serverSocketInfo;

        public void Startup()
        {
            // The chat server always starts up on the localhost, using the default port 
            IPHostEntry hostInfo = Dns.GetHostEntry(DEFAULT_SERVER);
            IPAddress serverAddr = hostInfo.AddressList[1];
            var serverEndPoint = new IPEndPoint(serverAddr, DEFAULT_PORT);
            // Create a listener socket and bind it to the endpoint 
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(serverEndPoint);

            Console.WriteLine("Server started at:" + serverSocket.LocalEndPoint.ToString());
        }

        public void Listen()
        {
            int backlog = 0;
            try
            {
                serverSocket.Listen(backlog);
                Console.WriteLine("Server listening");
                clientSocket = serverSocket.Accept();
                Console.WriteLine("Client connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to listen" + ex.ToString());
            }
        }

        public bool Disconnect()
        {
            clientSocket.Close();
            return true;
        }

        public void Run()
        {
            while (true)
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

        public bool SendData()
        {
            //pack.Add(value);
            Packet pack = new Packet(100);
            pack.Add(10);
            try
            {
                clientSocket.Send(pack.GetBytes());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }
    }
}
