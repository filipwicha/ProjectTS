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
        const int DEFAULT_PORT = 211;

        Mode mode = Mode.NotDefined;
        int currentSessionId;

        Socket clientSocket;
        bool isConnected = false;


        List<string> operands = new List<string>(new string[] { "+", "-", "*", "/", "x +", "log", "average", "==", "=",});
        string _equation = "";
        string equation
        {
            get
            {
                return _equation;
            }
            set
            {
                _equation = value + " ";
                displayMenu();
            }
        }

        public bool Connect()
        {
            IPAddress serverAddr = IPAddress.Parse("25.21.58.123");
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
            GetSessionId();

            return true;
        }

        public bool Disconnect()
        {
            clientSocket.Disconnect(false);
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
                else if(mode == Mode.TwoArguments)
                {
                    Console.WriteLine("One more operation?\n1.Yes\n2.No\n");
                    if (Convert.ToInt32(Console.ReadLine()) == 2)
                    {
                        return;
                    }
                }
            }
        }

        private void displayMenu()
        {
            Console.Clear();
            Console.WriteLine("Mode: " + mode.ToString());
            Console.WriteLine(_equation);
        }

        void GetSessionId()
        {
            byte[] buffer = new byte[256];
            try
            {
                var bytesrecd = clientSocket.Receive(buffer);
                Packet pack = new Packet(buffer);
                currentSessionId = pack.sessionId;
                Console.WriteLine("Session ID is set: " + currentSessionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
                    if (mode == Mode.MultiArguments)
                    {
                        Console.WriteLine("Actual result: " + pack.number1);
                    }
                    else
                    {
                        equation += pack.number1;
                    }
                    break;
                case State.DivisionByZero:
                    equation = "Division by 0";
                    break;
                case State.OverFlow:
                    equation = "Out of range! ";
                    break;
            }
        }

        private void SendData()
        {
            bool Error = true;
            do
            {
                Packet pack = new Packet();
                try
                {
                    if (mode == Mode.NotDefined)
                    {
                        Console.WriteLine("Choose the method:\n1.Two argument\n2.Multiargument\n");
                        mode = (Mode)Convert.ToInt32(Console.ReadLine()) - 1;
                        displayMenu();
                    }
                    if (mode == Mode.TwoArguments)
                    {
                        pack.mode = mode;
                        Console.Write("Give an operand x: ");
                        pack.number1 = Convert.ToInt32(Console.ReadLine());
                        equation = pack.number1.ToString();

                        Console.WriteLine("Choose operation:\n1.Addition\n2.Substraction\n3.Multiplication\n4.Division\n5.Linear Function\n6.Log\n7.Average\n8.Equals");
                        pack.operation = (Operation)Convert.ToInt32(Console.ReadLine()) - 1;
                        equation += operands[(int)pack.operation];

                        Console.Write("Give an operand y: ");
                        pack.number2 = Convert.ToInt32(Console.ReadLine());
                        equation += pack.number2 + " " + operands[8];
                    }
                    else if (mode == Mode.MultiArguments)
                    {
                        pack.mode = mode;

                        Console.Write("Give an operand x: ");
                        pack.number1 = Convert.ToInt32(Console.ReadLine());
                        equation += pack.number1;

                        pack.number2 = 0;

                        Console.WriteLine("Choose operation:\n1.Addition\n2.Substraction\n3.Multiplication\n4.Division\n5.Equals");
                        pack.operation = (Operation)Convert.ToInt32(Console.ReadLine()) - 1;

                        if (pack.operation == Operation.LinearFunction)
                        {
                            equation += operands[8];
                            pack.operation = Operation.Equals;
                            pack.mode = mode = Mode.MultiArgumentsLP;
                        }
                        else
                        {
                            equation += operands[(int)pack.operation];
                        }
                    }
                    pack.state = State.Nothing;
                    pack.sessionId = currentSessionId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    continue;
                }
                try
                {
                    clientSocket.Send(pack.GetBytes());
                    Error = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    isConnected = false;
                }
            } while (Error);
        }
    }
}
