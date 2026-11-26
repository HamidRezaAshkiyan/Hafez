# AGENTS.md — Hafez (حافظ)

## Overview
**Hafez** is a Windows desktop application (.NET 10 / WPF) for driving LED displays, projectors, and notice boards in mosques and religious ceremonies. It manages Quran, Mafatih, custom Duas, notifications, prayer times, Persian/Hijri calendars, live webcam/capture feeds, GPU-accelerated scrolling tickers, and remote wireless control from the companion Flutter app (`hafez_controller`) via an embedded ASP.NET Core Minimal API + SignalR server with QR-code pairing.

## Tech Stack & Requirements
- **Language / Runtime:** C# (`LangVersion=latest`) on **.NET 10.0 SDK** (`net10.0` for library/tests, `net10.0-windows` for WPF UI).
- **UI Framework:** WPF + Windows Forms (`UseWPF`, `UseWindowsForms`), `CommunityToolkit.Mvvm` 8.4, `MahApps.Metro` 2.4, `MaterialDesignThemes` 4.9, `QRCoder` 1.6.
- **Embedded Remote Server:** `Microsoft.AspNetCore.App` framework reference (Minimal API + SignalR hub at `/hub/hafez`).
- **Data & Storage:** Portable **SQLite** via `Microsoft.Data.Sqlite` + `Dapper` (migrated from legacy SQL Server LocalDB), Excel import/export via `ClosedXML`, JSON settings.
- **Media & Logging:** `OpenCvSharp4` + `OpenCvSharp4.runtime.win` + `OpenCvSharp4.WpfExtensions` for live camera capture; `Serilog` (Debug + rolling file sink).
- **Validation & Security:** `FluentValidation`, PBKDF2-SHA256 password hashing (`PasswordHasher`).
- **Testing:** `xUnit` 2.9 (`HafezLibrary.Tests`).

## Repository Structure
```text
Hafez/
├── Hafez.sln                       # Visual Studio solution
├── HafezLibrary/                   # Cross-platform core class library (net10.0)
│   ├── Controllers/                # Domain data controllers & DbCache (Quran, Mafatih, Dua, User, Notification)
│   ├── DataAccess/                 # DatabaseInitializer, SQLite connectors & processors
│   ├── Models/                     # Domain entities (QuranModel, MafatihModel, UserModel, etc.)
│   ├── Remote/RemoteModels.cs      # Shared SignalR/API DTOs & session token models
│   ├── Security/PasswordHasher.cs  # PBKDF2-SHA256 password hashing
│   ├── Services/                   # ContentCacheService, MosqueCalendarService
│   ├── Settings/AppSettingsService.cs # JSON app settings loader/saver
│   └── Validators/                 # FluentValidation validators
├── HafezWPFUI/                     # Windows WPF desktop app (net10.0-windows, v2.0.0)
│   ├── App.xaml / App.xaml.cs      # Entry point, DI container, Serilog, DB init, remote server startup
│   ├── Services/                   # HafezRemoteServer (ASP.NET Core + SignalR), ModernCameraService, DisplayLayoutService, ThemeService
│   ├── Helper/                     # NetCommandProcessor, Notification helpers, OperatorHotkeys
│   ├── ViewModels/                 # MVVM ViewModels (MainWindowViewModel, OutputWindowViewModel, handlers)
│   ├── Views/                      # XAML views & windows (MainWindowView, OutputWindowView, LivePreviewWindow, QrPairingWindow)
│   └── Resources/ & Gallery/       # Vazir/VazirFD Persian fonts, styles, sample logos
├── HafezLibrary.Tests/             # xUnit unit tests targeting HafezLibrary
├── HafezDataBase/                  # Legacy SQL Server .sqlproj schema/scripts (historical reference)
├── docs/index.html                 # GitHub Pages landing site
└── .github/workflows/ci.yml        # Windows CI build & test workflow
```

## Architecture & Key Flows
- **Startup (`HafezWPFUI/App.xaml.cs`):** Configures Serilog rolling logs under `%AppData%\Hafez\Logs\hafez-.log`, runs `DatabaseInitializer.EnsureInitialized()`, builds a `Microsoft.Extensions.DependencyInjection` service provider, warms up `IContentCacheService`, optionally auto-starts `HafezRemoteServer` on `settings.ApiPort` (default `5050`), applies saved dark/light theme via `ThemeService`, attaches `OperatorHotkeys` (`Ctrl+Q` QR pairing, `Ctrl+P` live preview, `Ctrl+D` theme toggle, `F11` secondary monitor), and shows `MainWindowView`.
- **Dual-Window Output:** `MainWindowView` acts as the operator control console while `OutputWindowView` renders the full-screen presentation output (text, prayer times, camera feed, smooth `TranslateTransform` ticker) intended for a secondary projector/LED display.
- **Remote Control (`HafezWPFUI/Services/HafezRemoteServer.cs`):** Hosts an embedded Kestrel server (`http://0.0.0.0:<ApiPort>`) exposing REST endpoints and SignalR hub `HafezHub` (`/hub/hafez`). Clients authenticate via a pairing token encoded in `GetPairingQrJson()` (`QrPairingWindow`) and invoke `SendCommand(RemoteCommandRequest)` while receiving `StateChanged` broadcasts.

## Development & Commands
```bash
# Restore projects
dotnet restore HafezLibrary/HafezLibrary.csproj
dotnet restore HafezWPFUI/HafezWPFUI.csproj
dotnet restore HafezLibrary.Tests/HafezLibrary.Tests.csproj

# Build library & tests (cross-platform)
dotnet build HafezLibrary/HafezLibrary.csproj -c Release
dotnet test HafezLibrary.Tests/HafezLibrary.Tests.csproj -c Release

# Build WPF UI (Windows; EnableWindowsTargeting=true allows cross-compile checks)
dotnet build HafezWPFUI/HafezWPFUI.csproj -c Release
```
Note: Do **not** run `dotnet build Hafez.sln` blindly on Linux/macOS without excluding `HafezDataBase.sqlproj`; build the individual `.csproj` files as in `.github/workflows/ci.yml`.

## Configuration & Environment
- **SQLite Database:** Stored at `%AppData%\Hafez\hafez.db` (initialized by `DatabaseInitializer.EnsureInitialized()`).
- **JSON Settings:** Stored at `%AppData%\Hafez\settings.json` (managed by `AppSettingsService`; includes `AutoStartApiServer`, `ApiPort` default `5050`, theme, etc.).
- **Logs:** `%AppData%\Hafez\Logs\hafez-.log` (daily rolling, 5 MB limit, 14 files retained).
- **Secrets:** Pairing tokens are generated at runtime by `RemoteSessionTokenService`; never hardcode or commit tokens or user credentials.

## Conventions & Gotchas
- **Windows-only runtime for UI:** `HafezWPFUI` requires Windows to run (WPF, WinForms, `OpenCvSharp4.runtime.win`), though `HafezLibrary` and `HafezLibrary.Tests` target pure `net10.0` and run cross-platform.
- **Legacy artifacts present:** `HafezDataBase/` (`.sqlproj`) and `HafezLibrary/Data/DB.mdf` are legacy SQL Server assets kept in-repo; v2.0 uses SQLite at runtime. Also `App_Start/**` in `HafezWPFUI` and `TinyxConnector.cs` / `TinyxConfig.cs` in `HafezLibrary` are explicitly excluded from compilation in `.csproj`.
- **RTL & Persian text:** UI strings and content are Persian/Arabic (`Vazir.ttf` / `VazirFD.ttf` bundled in `Resources/Fonts/`); searches normalize Arabic diacritics.
- **CI branch:** GitHub Actions triggers on `master` (not `main`) on `windows-latest`.
