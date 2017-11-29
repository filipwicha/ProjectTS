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
        
        public int result = 0;
        Operation operation = Operation.Addition;
        State state = State.Nothing;

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
                Packet pack = new Packet(buffer);

                if (pack.mode == Mode.TwoArguments)
                {
                    state = State.Nothing;
                    switch (pack.operation)
                    {
                        case Operation.Addition:
                            result = checked(pack.number1 + pack.number2);
                            break;
                        case Operation.Substraction:
                            result = checked(pack.number1 - pack.number2);
                            break;
                        case Operation.Multiplication:
                            result = checked(pack.number1 * pack.number2);
                            break;
                        case Operation.Division:
                            if (pack.number2 == 0)
                            {
                                state = State.DivisionByZero;
                                break;
                            }
                            result = checked(pack.number1 / pack.number2);
                            break;
                        case Operation.LinearFunction:
                            if (pack.number1 == 0)
                            {
                                state = State.DivisionByZero;
                                break;
                            }
                            result = checked((-pack.number2) / pack.number1);
                            break;
                        case Operation.Log:
                            result = checked(Convert.ToInt32(Math.Log(pack.number1, pack.number2)));
                            break;
                        case Operation.Average:
                            result = checked(((pack.number1 + pack.number2) / 2));
                            break;
                        case Operation.Equals:
                            if (pack.number1 == pack.number2) result = 1;
                            else result = 0;
                            break;
                    }
                }
                else if (pack.mode == Mode.MultiArguments || pack.mode == Mode.MultiArgumentsLP)
                {
                    switch (operation)
                    {
                        case Operation.Addition:
                            result = checked(result + pack.number1);
                            break;
                        case Operation.Substraction:
                            result = checked(result - pack.number1);
                            break;
                        case Operation.Multiplication:
                            result = checked(result * pack.number1);
                            break;
                        case Operation.Division:
                            if (pack.number1 == 0)
                            {
                                state = State.DivisionByZero;
                            }
                            else
                            {
                                result = checked( result / pack.number1);
                            }
                            break;
                    }
                    operation = pack.operation;
                }
            }
            catch (System.OverflowException e)
            {
                state = State.OverFlow;
                result = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Server received packet");
        }

        public void SendData()
        {
            Packet pack = new Packet();
            pack.operation = Operation.Addition;
            pack.number1 = result;
            pack.number2 = 0;
            pack.state = state;
            //pack.id = ...........;
            pack.mode = Mode.NotDefined;
            
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
