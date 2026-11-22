using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HafezLibrary.Controllers;
using HafezLibrary.Models;
using HafezLibrary.Remote;
using HafezLibrary.Services;
using HafezLibrary.Settings;
using HafezWPFUI.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TcpListenerHelper = HafezLibrary.DataAccess.Processor.TcpListener;

namespace HafezWPFUI.Services
{
    public sealed class HafezHub : Hub
    {
        private readonly HafezRemoteServer _remoteServer;

        public HafezHub(HafezRemoteServer remoteServer)
        {
            _remoteServer = remoteServer;
        }

        public override async Task OnConnectedAsync()
        {
            _remoteServer.OnClientConnected();
            await Clients.Caller.SendAsync("StateChanged", _remoteServer.CurrentState);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _remoteServer.OnClientDisconnected();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendCommand(RemoteCommandRequest request)
        {
            await _remoteServer.DispatchCommandAsync(request);
        }
    }

    /// <summary>
    /// Embedded ASP.NET Core Minimal API + SignalR server for modern mobile/web remote control.
    /// </summary>
    public sealed class HafezRemoteServer : IAsyncDisposable
    {
        private readonly IContentCacheService _contentCache;
        private readonly RemoteSessionTokenService _tokenService;
        private WebApplication _app;
        private int _connectedClients;

        public HafezLiveState CurrentState { get; } = new();
        public int Port { get; private set; } = 5050;
        public bool IsRunning => _app != null;
        public string LocalIp => TcpListenerHelper.GetLocalIpAddress();
        public string PairingToken => _tokenService.PairingToken;

        public HafezRemoteServer(IContentCacheService contentCache, RemoteSessionTokenService tokenService)
        {
            _contentCache = contentCache;
            _tokenService = tokenService;
        }

        public string GetPairingQrJson()
        {
            string companyName = GlobalConfig.UserConfig?.CompanyName;
            var payload = new PairingPayload(
                LocalIp,
                Port,
                _tokenService.PairingToken,
                string.IsNullOrWhiteSpace(companyName) ? "حافظ" : companyName);
            return JsonSerializer.Serialize(payload);
        }

        public async Task StartAsync(int? portOverride = null, CancellationToken cancellationToken = default)
        {
            if (_app != null)
            {
                return;
            }

            AppSettings settings = AppSettingsService.Load();
            Port = portOverride ?? (settings.ApiPort > 0 ? settings.ApiPort : 5050);

            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            builder.WebHost.UseUrls($"http://0.0.0.0:{Port}");

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true).AllowCredentials());
            });
            builder.Services.AddSignalR();
            builder.Services.AddSingleton(this);
            builder.Services.AddSingleton(_contentCache);
            builder.Services.AddSingleton(_tokenService);

            WebApplication app = builder.Build();
            app.UseCors();

            MapEndpoints(app);
            app.MapHub<HafezHub>("/hub/hafez");

            await app.StartAsync(cancellationToken);
            _app = app;

            Log.Information("Hafez Remote API & SignalR Hub started on http://{Ip}:{Port}", LocalIp, Port);
        }

        private void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/health", () => Results.Ok(new
            {
                status = "ok",
                app = "Hafez",
                version = "2.0.0",
                name = GlobalConfig.UserConfig?.CompanyName ?? "حافظ"
            }));

            app.MapPost("/api/auth/login", (LoginRequest req) =>
            {
                var candidate = new UserModel { UserId = req.UserId, Password = req.Password };
                UserModel valid = candidate.IsUserValid();
                if (valid == null)
                {
                    return Results.Unauthorized();
                }

                SessionInfo session = _tokenService.IssueSession(valid.Id, valid.UserId, valid.Name, valid.UserType);
                return Results.Ok(new LoginResponse(session.Token, session.UserId, session.Name, session.UserType.ToString()));
            });

            app.MapGet("/api/state", (HttpContext ctx) =>
            {
                if (!IsAuthorized(ctx, out _))
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(CurrentState);
            });

            app.MapGet("/api/catalog/surahs", async (HttpContext ctx) =>
            {
                if (!IsAuthorized(ctx, out _)) return Results.Unauthorized();
                return Results.Ok(await _contentCache.GetSureListAsync());
            });

            app.MapGet("/api/catalog/mafatih", async (HttpContext ctx) =>
            {
                if (!IsAuthorized(ctx, out _)) return Results.Unauthorized();
                return Results.Ok(await _contentCache.GetMafatihListAsync());
            });

            app.MapGet("/api/catalog/personal-duas", async (HttpContext ctx) =>
            {
                if (!IsAuthorized(ctx, out _)) return Results.Unauthorized();
                return Results.Ok(await _contentCache.GetPersonalDuaListAsync());
            });

            app.MapGet("/api/catalog/notification-groups", (HttpContext ctx) =>
            {
                if (!IsAuthorized(ctx, out _)) return Results.Unauthorized();
                return Results.Ok(NotificationGroupController.GetAllNotificationGroups());
            });

            app.MapGet("/api/catalog/gallery-folders", (HttpContext ctx) =>
            {
                if (!IsAuthorized(ctx, out _)) return Results.Unauthorized();
                string root = GlobalConfig.ImageLocation;
                string[] folders = Directory.Exists(root)
                    ? Directory.GetDirectories(root).Select(Path.GetFileName).ToArray()
                    : Array.Empty<string>();
                return Results.Ok(folders);
            });

            app.MapPost("/api/notifications/send", async (HttpContext ctx, SendNotificationRequest req) =>
            {
                if (!IsAuthorized(ctx, out SessionInfo session)) return Results.Unauthorized();
                if (string.IsNullOrWhiteSpace(req.Description))
                {
                    return Results.BadRequest(new { error = "Description is required." });
                }

                var model = new NotificationModel
                {
                    GroupId = req.GroupId > 0 ? req.GroupId : 2,
                    Name = req.Title ?? string.Empty,
                    Description = req.Description,
                    NotificationType = 'N',
                    CreatedBy = session.UserIdDb
                };
                NotificationController.CreateNotification(model);

                CurrentState.ActiveNotificationText = req.Description;
                CurrentState.UpdatedAtUtc = DateTime.UtcNow;

                await BroadcastStateAsync();
                if (_app != null)
                {
                    var hub = _app.Services.GetRequiredService<IHubContext<HafezHub>>();
                    await hub.Clients.All.SendAsync("NotificationSent", model.Name, model.Description, req.Urgent);
                }

                return Results.Ok(model);
            });

            app.MapPost("/api/commands", async (HttpContext ctx, RemoteCommandRequest req) =>
            {
                if (!IsAuthorized(ctx, out _)) return Results.Unauthorized();
                await DispatchCommandAsync(req);
                return Results.Ok(CurrentState);
            });
        }

        private bool IsAuthorized(HttpContext ctx, out SessionInfo session)
        {
            string header = ctx.Request.Headers.Authorization.ToString();
            string token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? header["Bearer ".Length..].Trim()
                : ctx.Request.Query["access_token"].ToString();

            return _tokenService.TryValidate(token, out session);
        }

        public async Task DispatchCommandAsync(RemoteCommandRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Target))
            {
                return;
            }

            string target = req.Target.Trim().ToLowerInvariant();
            string action = (req.Action ?? string.Empty).Trim().ToLowerInvariant();

            UpdateStateFromCommand(target, action, req.Value);

            string legacyCommand = string.IsNullOrWhiteSpace(req.Value)
                ? $"{target}/{action}"
                : $"{target}/{action}/{req.Value}";

            if (Application.Current?.Dispatcher != null)
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    try
                    {
                        var syntheticUser = new UserModel
                        {
                            Id = 1,
                            UserId = "api",
                            Name = "کنترلر موبایل",
                            Statue = 'E',
                            UserType = 'A',
                            Command = legacyCommand
                        };
                        await NetCommandProcessor.DoCommandAsync(syntheticUser);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Remote command execution error for {Command}", legacyCommand);
                    }
                });
            }

            await BroadcastStateAsync();
        }

        private void UpdateStateFromCommand(string target, string action, string value)
        {
            switch (target)
            {
                case "notification":
                    if (action == "play") CurrentState.IsNotificationPlaying = true;
                    if (action is "stop" or "pause") CurrentState.IsNotificationPlaying = false;
                    break;
                case "quran":
                    if (action == "play") CurrentState.IsQuranPlaying = true;
                    if (action is "stop" or "pause") CurrentState.IsQuranPlaying = false;
                    if (action == "select" && int.TryParse(value, out int page)) CurrentState.ActiveQuranPage = page;
                    break;
                case "mafatih":
                    if (action == "play") CurrentState.IsMafatihPlaying = true;
                    if (action is "stop" or "pause") CurrentState.IsMafatihPlaying = false;
                    break;
                case "personaldua":
                    if (action == "play") CurrentState.IsPersonalDuaPlaying = true;
                    if (action is "stop" or "pause") CurrentState.IsPersonalDuaPlaying = false;
                    break;
                case "camera":
                    if (action is "play" or "start") CurrentState.IsCameraActive = true;
                    if (action is "stop" or "pause") CurrentState.IsCameraActive = false;
                    break;
                case "gallery":
                    if (action == "play") CurrentState.IsGalleryPlaying = true;
                    if (action is "stop" or "pause") CurrentState.IsGalleryPlaying = false;
                    break;
            }

            CurrentState.UpdatedAtUtc = DateTime.UtcNow;
        }

        internal void OnClientConnected()
        {
            CurrentState.ConnectedClients = Interlocked.Increment(ref _connectedClients);
            _ = BroadcastStateAsync();
        }

        internal void OnClientDisconnected()
        {
            CurrentState.ConnectedClients = Math.Max(0, Interlocked.Decrement(ref _connectedClients));
            _ = BroadcastStateAsync();
        }

        public async Task BroadcastStateAsync()
        {
            if (_app == null)
            {
                return;
            }

            try
            {
                var hub = _app.Services.GetRequiredService<IHubContext<HafezHub>>();
                await hub.Clients.All.SendAsync("StateChanged", CurrentState);
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to broadcast SignalR state");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_app != null)
            {
                await _app.StopAsync();
                await _app.DisposeAsync();
                _app = null;
            }
        }
    }
}
