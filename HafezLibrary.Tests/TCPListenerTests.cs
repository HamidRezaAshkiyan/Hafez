using Xunit;
using static HafezLibrary.DataAccess.Processor.TcpListener;

namespace HafezLibrary.Tests
{
    public class TCPListenerTests
    {
        [Fact]
        public void GetLocalIpAddress_ShouldWork()
        {
            //arrange
            string expected = "192.168.1.24";

            //act
            string actual = GetLocalIpAddress();

            //assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLocalIpAddress_WithVPNShouldNotWork()
        {
            //arrange
            string expected = "192.168.1.24";

            //act
            string actual = GetLocalIpAddress();

            //assert
            Assert.Equal(expected, actual);
        }
    }
}