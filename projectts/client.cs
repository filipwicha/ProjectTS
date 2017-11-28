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

        public string flag0 = "n"; //informs about not chosen operation (n), dualoperand (d), multioperand (m), multioperandls(mls) 
        public bool flag1 = false; //informs about multioperand operation (0 -not last sent, 1 -last sent)

        bool isConnected = false;

        //Client socket stuff 
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
            int oneloop = 0; //loop once again and stop
            while (isConnected && oneloop != 1)
            while (isConnected)
            {
                SendData();
                ReceiveData();
                if(flag1 == true)
                {
                    oneloop++;
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
                case Nothing:
                    Console.WriteLine("Actual result = " + pack.number1);
                    break;
                case DivisionByZero:
                    Console.WriteLine("Division by 0");
                    break;
                case Nothing:
                    Console.WriteLine("Out of range" + pack.number1);
                    break;
                case Nothing:
                    Console.WriteLine("Actual result = " + pack.number1);
                    break;
            }
            Packet packet = new Packet(buffer);
            Console.WriteLine("Client received data");
        }


        private void SendData()
        {
            Packet pack = new Packet(100);
            string temp;

            if (flag0 == "n")
            {
                Console.Write("Choose the method (d - dual operand calculation, m - multi operand calculation): ");
                Console.ReadLine(temp);
                if (temp == "d") //dualoperand
                {
                    flag0 = "d";
                    pack.mode = TwoArguments;
                    Console.Write("Give an operand x: ");
                    pack.number1 = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Give an operand y: ");
                    pack.number2 = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Give an operator (add, sub, mul, div, sqrt, log, avg, eqals: ");
                    Console.ReadLine(temp);
                    switch(temp)
                    {
                        case "add":
                            pack.operation = Addition;
                            break;
                        case "sub":
                            pack.operation = Substraction;
                            break;
                        case "mul":
                            pack.operation = Multiplication;
                            break;
                        case "div":
                            pack.operation = Division;
                            break;
                        case "sqrt":
                            pack.operation = Sqrt;
                            break;
                        case "log":
                            pack.operation = Log;
                            break;    
                        case "avg":
                            pack.operation = Average;
                            break;
                        case "egals":
                            pack.operation = Equals;
                            break;
                    }
                }
                else if (temp == "m") //multioperand
                {
                flag0 = "m";
                pack.mode = MultiArguments;
                Console.Write("Give an operand x: ");
                pack.number1 = Convert.ToInt32(Console.ReadLine());
                pack.number2 = 0;
                Console.Write("Give an operator (add, sub, mul, div, eqals (if the last operand): ");
                Console.ReadLine(temp);
                switch(temp)
                    {
                    case "add":
                        pack.operation = Addition;
                        break;
                    case "sub":
                        pack.operation = Substraction;
                        break;
                    case "mul":
                        pack.operation = Multiplication;
                        break;
                    case "div":
                        pack.operation = Division;
                        break;
                    case "eqals":
                        pack.operation = Equals;
                        flag1 = true;
                        break;
                    }
                }
            }





            else
            {
                if (flag0 == "m") //multioperand
                {
                pack.mode = MultiArguments;
                Console.Write("Give an operand x: ");
                pack.number1 = Convert.ToInt32(Console.ReadLine());
                pack.number2 = 0;
                Console.Write("Give an operator (add, sub, mul, div, eqals (if the last operand)): ");
                Console.ReadLine(temp);
                switch(temp)
                    {
                    case "add":
                        pack.operation = Addition;
                        break;
                    case "sub":
                        pack.operation = Substraction;
                        break;
                    case "mul":
                        pack.operation = Multiplication;
                        break;
                    case "div":
                        pack.operation = Division;
                        break;
                    case "eqals":
                        pack.operation = Equals;
                        pack.mode = MultiArgumentsLP;
                        flag1 = true;
                        break;
                    }
                }
            }
            pack.state = Nothing;
            //ustalanie id
            pack.Serialize();

//=======
            Packet pack = new Packet();
            Console.WriteLine("Send number");
//>>>>>>> ceb62c61b28e6ce024e217973884b8290d21b82d
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
