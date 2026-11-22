using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace HafezLibrary.Remote
{
    public record RemoteCommandRequest(
        string Target,
        string Action,
        string Value = null,
        string SecondaryValue = null);

    public record SendNotificationRequest(
        string Title,
        string Description,
        int GroupId = 2,
        bool Urgent = false);

    public record LoginRequest(string UserId, string Password);

    public record LoginResponse(string Token, string UserId, string Name, string UserType);

    public record PairingPayload(string Host, int Port, string Token, string Name);

    public class HafezLiveState
    {
        public bool IsNotificationPlaying { get; set; }
        public bool IsQuranPlaying { get; set; }
        public bool IsMafatihPlaying { get; set; }
        public bool IsPersonalDuaPlaying { get; set; }
        public bool IsCameraActive { get; set; }
        public bool IsGalleryPlaying { get; set; }
        public string ActiveNotificationText { get; set; } = string.Empty;
        public string ActiveQuranTitle { get; set; } = string.Empty;
        public int ActiveQuranPage { get; set; } = 1;
        public string ActiveMafatihTitle { get; set; } = string.Empty;
        public string PrayerBanner { get; set; } = string.Empty;
        public int ConnectedClients { get; set; }
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }

    public record SessionInfo(string Token, int UserIdDb, string UserId, string Name, char UserType, DateTime CreatedAtUtc);

    /// <summary>
    /// Manages session tokens and the active QR pairing token for local Wi-Fi controllers.
    /// </summary>
    public sealed class RemoteSessionTokenService
    {
        private readonly ConcurrentDictionary<string, SessionInfo> _sessions = new();

        public string PairingToken { get; private set; }

        public RemoteSessionTokenService()
        {
            PairingToken = GenerateToken();
            _sessions[PairingToken] = new SessionInfo(PairingToken, 1, "qr-operator", "اپراتور موبایل", 'A', DateTime.UtcNow);
        }

        public string RotatePairingToken()
        {
            if (!string.IsNullOrEmpty(PairingToken))
            {
                _sessions.TryRemove(PairingToken, out _);
            }

            PairingToken = GenerateToken();
            _sessions[PairingToken] = new SessionInfo(PairingToken, 1, "qr-operator", "اپراتور موبایل", 'A', DateTime.UtcNow);
            return PairingToken;
        }

        public SessionInfo IssueSession(int userIdDb, string userId, string name, char userType)
        {
            string token = GenerateToken();
            var session = new SessionInfo(token, userIdDb, userId, name, userType, DateTime.UtcNow);
            _sessions[token] = session;
            return session;
        }

        public bool TryValidate(string token, out SessionInfo session)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                session = null;
                return false;
            }

            return _sessions.TryGetValue(token, out session);
        }

        public bool Revoke(string token) => !string.IsNullOrWhiteSpace(token) && _sessions.TryRemove(token, out _);

        private static string GenerateToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(24)).ToLowerInvariant();
        }
    }
}
