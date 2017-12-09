using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTS
{
    class Program
    {
        static Server server; //create server
        static Client client; //create client
        
        static void Main(string[] args)
        {
            Console.WriteLine("Start as:\n1.Client\n2.Server"); //menu that allows to choose if the program is client or server
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
            }
            Console.ReadLine();
        }
    }
}
