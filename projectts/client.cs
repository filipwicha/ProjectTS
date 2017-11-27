using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTS
{
    public class Client
    {
        const string DEFAULT_SERVER = "localhost";
        const int DEFAULT_PORT = 804;

        bool isConnected = false;

        //Client socket stuff 
        Socket clientSocket;
        Socket serverSocket;
        SocketInformation clientSocketInfo;

        public bool Connect()
        {
            // The chat client always starts up on the localhost, using the default port 
            IPHostEntry hostInfo = Dns.GetHostEntry(DEFAULT_SERVER);
            IPAddress serverAddr = hostInfo.AddressList[1];
            var clientEndPoint = new IPEndPoint(serverAddr, DEFAULT_PORT);

            // Create a client socket and connect it to the endpoint 
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Try to connect to server (Timeout = 30s)
            int Timeout = 6;
            while(Timeout>0){
                try
                {
                    clientSocket.Connect(clientEndPoint);
                    Timeout = 0;
                    isConnected = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Timeout--;
                    System.Threading.Thread.Sleep(5000);
                    if (Timeout == 0)
                    {
                        Console.WriteLine("Connection canceled due to timeout");
                        return false;
                    }
                }
            }
            Console.WriteLine("Connected to " + clientSocket.LocalEndPoint.ToString());
            return true;
        }

        public bool Disconnect()
        {
            clientSocket.Close();
            return true;
        }

        public void Run()
        {
            while (isConnected)
            {
                SendData();
                ReceiveData();
            }
        }

        private void ReceiveData()
        {
            byte[] buffer = new byte[256];

            var bytesrecd = clientSocket.Receive(buffer);

            Console.WriteLine("Client received data");
        }

        private void SendData()
        {
            Packet pack = new Packet(100);
            Console.WriteLine("Send number");
            pack.Add(Convert.ToInt32(Console.ReadLine()));
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
