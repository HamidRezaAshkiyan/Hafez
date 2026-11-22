using System.Text.Json;
using HafezLibrary.Remote;
using Xunit;

namespace HafezLibrary.Tests
{
    public class RemoteApiTests
    {
        [Fact]
        public void RemoteSessionTokenService_ShouldValidatePairingAndIssuedTokens()
        {
            var service = new RemoteSessionTokenService();

            Assert.True(service.TryValidate(service.PairingToken, out SessionInfo pairingSession));
            Assert.Equal("qr-operator", pairingSession.UserId);

            SessionInfo userSession = service.IssueSession(2, "ali", "علی", 'A');
            Assert.True(service.TryValidate(userSession.Token, out SessionInfo validated));
            Assert.Equal("ali", validated.UserId);

            Assert.True(service.Revoke(userSession.Token));
            Assert.False(service.TryValidate(userSession.Token, out _));

            string oldPairing = service.PairingToken;
            string newPairing = service.RotatePairingToken();
            Assert.NotEqual(oldPairing, newPairing);
            Assert.False(service.TryValidate(oldPairing, out _));
            Assert.True(service.TryValidate(newPairing, out _));
        }

        [Fact]
        public void PairingPayload_ShouldSerializeCleanlyForMobileQrScanner()
        {
            var payload = new PairingPayload("192.168.1.50", 5050, "abc123", "مسجد حافظ");
            string json = JsonSerializer.Serialize(payload);
            PairingPayload roundTripped = JsonSerializer.Deserialize<PairingPayload>(json);

            Assert.NotNull(roundTripped);
            Assert.Equal("192.168.1.50", roundTripped.Host);
            Assert.Equal(5050, roundTripped.Port);
            Assert.Equal("abc123", roundTripped.Token);
            Assert.Equal("مسجد حافظ", roundTripped.Name);
        }
    }
}
