using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HafezLibrary.DataAccess.Processor;
using Xunit;

namespace HafezLibrary.Tests
{
    public class TCPListenerTests
    {
        [Fact]
        public void GetLocalIpAddress_ShouldReturnValidIPv4()
        {
            string actual = HafezLibrary.DataAccess.Processor.TcpListener.GetLocalIpAddress();

            Assert.True(IPAddress.TryParse(actual, out IPAddress parsed));
            Assert.Equal(AddressFamily.InterNetwork, parsed.AddressFamily);
        }

        [Fact]
        public async Task StartListening_ShouldReadPayloadLargerThan512BytesWithoutTruncation()
        {
            var server = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
            server.Start();
            try
            {
                int port = ((IPEndPoint)server.LocalEndpoint).Port;
                string expected = "admin/secret/notification/send/title/" + new string('A', 2048);

                Task<string> listenTask = HafezLibrary.DataAccess.Processor.TcpListener.StartListening(server);

                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(IPAddress.Loopback, port);
                    byte[] payload = Encoding.UTF8.GetBytes(expected);
                    await client.GetStream().WriteAsync(payload);
                }

                string received = await listenTask;
                Assert.Equal(expected, received);
            }
            finally
            {
                server.Stop();
            }
        }

        [Theory]
        [InlineData("admin/pass/notification/play/2", true, "admin", "pass", "notification/play/2")]
        [InlineData("admin/pass/camera/start", true, "admin", "pass", "camera/start")]
        [InlineData("", false, "", "", "")]
        [InlineData("malformed_no_slashes", false, "", "", "")]
        [InlineData("only/two", false, "", "", "")]
        public void TryParseCommandPacket_ShouldHandleValidAndMalformedPackets(
            string raw, bool expectedOk, string expectedUser, string expectedPass, string expectedCommand)
        {
            bool ok = HafezLibrary.DataAccess.Processor.TcpListener.TryParseCommandPacket(
                raw, out string user, out string pass, out string cmd);

            Assert.Equal(expectedOk, ok);
            Assert.Equal(expectedUser, user);
            Assert.Equal(expectedPass, pass);
            Assert.Equal(expectedCommand, cmd);
        }
    }
}
