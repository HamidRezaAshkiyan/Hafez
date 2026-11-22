using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HafezLibrary.DataAccess.Processor
{
    public static class TcpListener
    {
        private const int MaxPayloadBytes = 64 * 1024; // 64 KB safety limit

        public static string GetLocalIpAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                {
                    return ip.ToString();
                }
            }

            return IPAddress.Loopback.ToString();
        }

        public static System.Net.Sockets.TcpListener GetServer(IPAddress localAddress, int portNumber)
        {
            return new System.Net.Sockets.TcpListener(localAddress, portNumber);
        }

        /// <summary>
        /// Accepts a single client connection on a started TCP listener and reads the full UTF-8 payload.
        /// </summary>
        public static async Task<string> StartListening(
            System.Net.Sockets.TcpListener server,
            CancellationToken cancellationToken = default)
        {
            using TcpClient client = await server.AcceptTcpClientAsync(cancellationToken);
            using NetworkStream stream = client.GetStream();
            using MemoryStream bufferStream = new();

            byte[] chunk = new byte[1024];
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(chunk.AsMemory(0, chunk.Length), cancellationToken)) > 0)
            {
                if (bufferStream.Length + bytesRead > MaxPayloadBytes)
                {
                    throw new InvalidDataException($"Incoming TCP message exceeded {MaxPayloadBytes} bytes.");
                }

                bufferStream.Write(chunk, 0, bytesRead);
            }

            return Encoding.UTF8.GetString(bufferStream.ToArray());
        }

        /// <summary>
        /// Safely parses a legacy "userId/password/command..." packet without throwing on malformed input.
        /// </summary>
        public static bool TryParseCommandPacket(string rawPacket, out string userId, out string password, out string command)
        {
            userId = string.Empty;
            password = string.Empty;
            command = string.Empty;

            if (string.IsNullOrWhiteSpace(rawPacket))
            {
                return false;
            }

            string[] parts = rawPacket.Split('/', 3);
            if (parts.Length < 3 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[2]))
            {
                return false;
            }

            userId = parts[0];
            password = parts[1];
            command = parts[2];
            return true;
        }
    }
}
