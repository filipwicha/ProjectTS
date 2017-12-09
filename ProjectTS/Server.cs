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
        const int DEFAULT_PORT = 211;

        bool isConnected = false;

        //Sockets
        Socket serverSocket;
        Socket clientSocket;

        int currentSessionId; //flag that informs about current session Id
        public int result = 0; //variable that holds current result of calculation
        Operation operation = Operation.Addition;   // first operation is set to addition, 
        State state = State.Nothing;                //because the first number form first packet must be set as current result

        public void Startup()
        {
            IPAddress serverAddr = IPAddress.Parse("127.0.0.1"); //set client's IP
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //create server socket
            serverSocket.Bind(new IPEndPoint(serverAddr, DEFAULT_PORT)); //bind

            Console.WriteLine("Server started at:" + serverSocket.LocalEndPoint.ToString());

            int backlog = 0; //how many pending connections the queue will hold

            try
            {
                serverSocket.Listen(backlog); //start listening
                Console.WriteLine("Server listening");
                clientSocket = serverSocket.Accept(); //accept connection
                isConnected = true;
                Console.WriteLine("Client connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to listen" + ex.ToString());
            }
            SetSessionId(); //set sessionId

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

        void SetSessionId()
        {
            Packet pack = new Packet(); //create packet with Id to be set by client
            pack.sessionId = currentSessionId = 7; //set Id to 7
            clientSocket.Send(pack.GetBytes()); //serialize and send
            Console.WriteLine("Session ID is set: " + currentSessionId);
        }


        public void ReceiveData()
        {
            byte[] buffer = new byte[256];
            try
            {
                var bytesrecd = clientSocket.Receive(buffer); //receive data from client
                Packet pack = new Packet(buffer); //deserialize and create packet with received data

                if (pack.mode == Mode.TwoArguments) //if mode is two arguments, calculate with chosen operation
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
                else if (pack.mode == Mode.MultiArguments || pack.mode == Mode.MultiArgumentsLP) //if mode is multi arguments, calculate with chosen operation
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
                    if(pack.operation == Operation.Equals)
                    {
                        isConnected = false;
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
            Packet pack = new Packet(); //create empty packet
            pack.operation = Operation.Addition; //set any operation
            pack.number1 = result; //first number in packet will be the result of calculation to this point
            pack.number2 = 0; //set any second number
            pack.state = state; //set state to that from flag
            pack.sessionId = currentSessionId; //set Id from flag
            pack.mode = Mode.NotDefined; //set any mode
            
            try
            {
                clientSocket.Send(pack.GetBytes()); //serialize and send
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                isConnected = false;
            }
        }
    }
}
