using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTS
{
    class Program
    {
        static Server server;
        static Client client;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Start as:\n1.Client\n2.Server");
            if(Convert.ToInt32(Console.ReadLine()) == 1)
            {
                client = new Client();
                if (client.Connect())
                {
                    client.Run();
                    client.Disconnect();
                }
            }
            else
            {
                server = new Server();
                server.Startup();
                server.Listen();
                server.Run();
            }
            Console.ReadLine();
            //exit 
        }
    }
}
