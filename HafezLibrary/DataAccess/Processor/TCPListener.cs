using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HafezLibrary.DataAccess.Processor
{
    public static class TcpListener
    {
        public static string GetLocalIpAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach ( IPAddress ip in host.AddressList )
            {
                if ( ip.AddressFamily == AddressFamily.InterNetwork )
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static System.Net.Sockets.TcpListener GetServer(IPAddress localAddress, int portNumber)
        {
            return new System.Net.Sockets.TcpListener(localAddress, portNumber);
        }

        /// <summary>
        ///     listening to STARTED server
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static async Task<string> StartListening(System.Net.Sockets.TcpListener server)
        {
            // Buffer for reading data
            byte[] bytes = new byte[512];

            // Perform a blocking call to accept requests.
            TcpClient client = await server.AcceptTcpClientAsync();
            //Console.WriteLine("Connected!");

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int    i;
            string portOutputText = "";
            // Loop to receive all the data sent by the client.
            while ( (i = stream.Read(bytes, 0, bytes.Length)) != 0 )
                // Translate data bytes to a UTF8 string.
            {
                portOutputText = Encoding.UTF8.GetString(bytes, 0, i);
            }


            // Shutdown and end connection
            client.Close();

            //server.Stop();

            return portOutputText;
        }
    }
}