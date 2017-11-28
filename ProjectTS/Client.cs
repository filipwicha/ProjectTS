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

        Mode mode = Mode.NotDefined;
        
        //public bool flag1 = false; //informs about multioperand operation (0 -not last sent, 1 -last sent)


        Socket clientSocket;
        bool isConnected = false;
        
        public bool Connect()
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(DEFAULT_SERVER);
            IPAddress serverAddr = hostInfo.AddressList[1];
            var clientEndPoint = new IPEndPoint(serverAddr, DEFAULT_PORT);

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Try to connect to server (Timeout = 30s)
            int Timeout = 6;
            while(Timeout>0){
                try
                {
                    clientSocket.Connect(clientEndPoint);
                    isConnected = true;
                    Timeout = 0;
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
                if(mode == Mode.MultiArgumentsLP)
                {
                    return;
                }
            }
        }

        

        private void ReceiveData()
        {
            byte[] buffer = new byte[256];
            var bytesrecd = clientSocket.Receive(buffer);
            Packet pack = new Packet(buffer);
            switch (pack.state)
            {
                case State.Nothing:
                    equation += pack.number1;
                    break;
                case State.DivisionByZero:
                    equation = "Division by 0";
                    break;
                case State.OutOfRange:
                    equation = "Out of range" + pack.number1;
                    break;
            }
        }

        private void SendData()
        {
            Packet pack = new Packet();
            if (mode == Mode.NotDefined)
            {
                Console.Write("");
                Console.WriteLine("Choose the method:\n1.Two argument\n2.Multiargument\n");
                mode = (Mode)Convert.ToInt32(Console.ReadLine()) - 1;
                displayMenu();
            }
            if (mode == Mode.TwoArguments)
            {
                pack.mode = mode;
                Console.Write("Give an operand x: ");
                
                pack.number1 = Convert.ToInt32(Console.ReadLine());
                equation += pack.number1;

                Console.WriteLine("Choose operation:\n1.Addition\n2.Substraction\n3.Multiplication\n4.Division\n5.Linear Function\n6.Log\n7.Average\n8.Equals");
                pack.operation = (Operation)Convert.ToInt32(Console.ReadLine()) - 1;

                Console.Write("Give an operand y: ");
                pack.number2 = Convert.ToInt32(Console.ReadLine());
                equation += pack.number2+ " =";
            }
            else if (mode == Mode.MultiArguments)
            {
                pack.mode = mode;
                Console.Write("Give an operand x: ");
                pack.number1 = Convert.ToInt32(Console.ReadLine());
                pack.number2 = 0;
                Console.WriteLine("Choose operation:\n1.Addition\n2.Substraction\n3.Multiplication\n4.Division\n5.Equals");
                pack.operation = (Operation)Convert.ToInt32(Console.ReadLine())-1;
                if (pack.operation == Operation.LinearFunction)
                {
                    pack.operation = Operation.Equals;
                    pack.mode = mode = Mode.MultiArgumentsLP;
                }
            }
            pack.state = State.Nothing;
            //ustalanie id

            Console.WriteLine("Send number");
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
    public class Menu
    {
        public Mode mode;
        public Operation operation;
        string _equation = " ";
        public string equation
        {
            get
            {
                StringBuilder sb = new StringBuilder();
            }
            set
            {
                _equation = value + " ";
                Display();
            }
        }

        List<string> operands = new List<string>(new string[] { " + ", " - ", " * ", " / ", " - ", " ", " ", " ",});

        private void Display()
        {
            Console.Clear();
            Console.WriteLine("Mode: " + mode.ToString());
            Console.WriteLine(_equation);
        }
    }
}
