﻿using System;
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
        
        public int buff = 0;
        public int op = 1; //(1 - dodawanie, 2 - odejmowanie, 3 - mnozenie, 4 - dzielenie, 5 - rowna się)
        public int st = 0; //0-nothing, 1 divby0, 2 outofrange, 3 notdefined,

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
                Packet pack = new Packet(buffer);

                if(pack.mode = TwoArguments)
                {
                    switch (pack.operation)
                    {
                        case Addition:
                            buff = pack.number1 + pack.number2;
                            break;
                        case Substraction:
                            buff = pack.number1 - pack.number2;
                            break;
                        case Multiplication:
                            buff = pack.number1 * pack.number2;
                            break;
                        case Division:
                            if (pack.number2==0)
                            {
                                pack.state = DivisionByZero;
                                break;
                            }
                            buff = pack.number1 / pack.number2;
                            break;
                        case Sqrt:
                            buff = Convert.ToInt32(Math.Sqrt(pack.number1, pack.number2));
                            break;
                        case Log:
                            buff = Convert.ToInt32(Math.Log(pack.number1, pack.number2));
                            break;
                        case Average:
                            buff = ((pack.number1 + pack.number2)/2);
                            break;
                        case Equals:
                            if (pack.number1 == pack.number2) buff = 1;
                            else buff = 0;
                            break;
                    }
                }
                else if (pack.mode == MultiArguments)
                {
                    if (op == 1)
                    {
                        buff += pack.number1;
                        switch(pack.operation)
                        {
                            case Addition:
                                op = 1;
                                break;
                            case Substraction:
                                op = 2;
                                break;
                            case Multiplication:
                                op = 3;
                                break;
                            case Division:
                                op = 4;
                                break;
                            case Equals:
                                pack.mode = MultiArgumentsLP;
                                op = 5;
                                break;
                        }
                    }
                    if (op == 2)
                    {
                        buff -= pack.number1;
                        switch(pack.operation)
                        {
                            case Addition:
                                op = 1;
                                break;
                            case Substraction:
                                op = 2;
                                break;
                            case Multiplication:
                                op = 3;
                                break;
                            case Division:
                                op = 4;
                                break;
                            case Equals:
                                pack.mode = MultiArgumentsLP;
                                op = 5;
                                break;
                        }

                    }
                    if (op == 3)
                    {
                        buff *= pack.number1;
                        switch(pack.operation)
                        {
                            case Addition:
                                op = 1;
                                break;
                            case Substraction:
                                op = 2;
                                break;
                            case Multiplication:
                                op = 3;
                                break;
                            case Division:
                                op = 4;
                                break;
                            case Equals:
                                pack.mode = MultiArgumentsLP;
                                op = 5;
                                break;
                        }
                    }
                    if (op == 4)
                    {
                        if(pack.number2 == 0)
                        {
                            st = 1;
                        }
                        else
                        {
                            buff /= pack.number1;
                            switch(pack.operation)
                        {
                            case Addition:
                                op = 1;
                                break;
                            case Substraction:
                                op = 2;
                                break;
                            case Multiplication:
                                op = 3;
                                break;
                            case Division:
                                op = 4;
                                break;
                            case Equals:
                                op = 5;
                                pack.mode = MultiArgumentsLP;
                                break;
                        }
                        }
                    }
                }
                else if (pack.mode == MultiArgumentsLP)
                {
                    
                }












                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Server received packet");
        }

        public bool SendData()
        {
            Packet pack = new Packet(100);
            pack.operation = Addition;
            pack.number1 = buff;
            pack.number2 = 0;
            switch(st)
            {
                case 0:
                    pack.state = Nothing;
                    break;
                case 1:
                    pack.state = DivisionByZero;
                    break;
                case 2:
                    pack.state = OutOfRange;
                    break;
                case 3:
                    pack.state = NotDefined;
                    break;
                
            }
            //pack.id = ...........;
            pack.mode = NotDefined;


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
