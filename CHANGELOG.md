# Changelog

## v2.0.0 (2026-11-22)

### 🚀 Platform & Architecture
- Upgraded `HafezLibrary`, `HafezWPFUI`, and `HafezLibrary.Tests` to **.NET 10** (`net10.0` / `net10.0-windows`).
- Replaced SQL Server LocalDB (`System.Data.SqlClient`) with portable **SQLite** (`Microsoft.Data.Sqlite`) stored in `%AppData%\Hafez\hafez.db` with WAL mode and automatic schema/seed initialization.
- Added versioned JSON settings persisted to `%AppData%\Hafez\settings.json` (`AppSettingsService`).
- Replaced legacy Excel COM Interop with **ClosedXML** (zero Microsoft Office dependency).
- Replaced `Caliburn.Micro` and `AutoMapper` with **CommunityToolkit.Mvvm** and **Microsoft.Extensions.DependencyInjection**.
- Replaced synchronous `DbCache` loading with async `ContentCacheService` including Arabic/Persian diacritic-tolerant search.
- Replaced unmaintained `AForge.Video.DirectShow` with **OpenCvSharp4** (`ModernCameraService`).
- Added GPU-accelerated `SmoothMarqueeAnimator` (`TranslateTransform`).

### 📡 Remote Control & Pairing
- Embedded an **ASP.NET Core Minimal API + SignalR Hub** (`/hub/hafez`) for real-time two-way communication with `hafez_controller`.
- Added bearer session tokens and QR code pairing (`QrPairingWindow`).

### 🕌 Mosque UX & Productivity
- Added `MosqueCalendarService` (Tehran method prayer times, next-prayer countdown, Jalali & Hijri dates, religious occasions).
- Added runtime **Dark Mode** (`ThemeService`), **Live Output Preview** (`LivePreviewWindow`), secondary monitor auto-snap (`DisplayLayoutService`), and operator keyboard shortcuts (`Ctrl+Q`, `Ctrl+P`, `Ctrl+D`, `F11`).

### 🔒 Security & Bug Fixes
- Added PBKDF2-SHA256 password hashing (`PasswordHasher`) with automatic legacy plaintext migration.
- Fixed `NullReferenceException` on failed TCP logins, malformed packet crashes, and 512-byte TCP buffer truncation.
- Fixed SQL injection in `UserConfigModel.Update`.
- Removed retired `Microsoft.AppCenter` SDK & hardcoded secret, and removed hardware dongle check blocking open-source Release builds.

### 🧪 Testing, Docs & CI/CD
- Converted `HafezLibrary.Tests` to SDK-style xUnit with unit tests covering security, TCP listener, SQLite CRUD, JSON settings, ClosedXML, SignalR token auth, text normalization, and prayer times.
- Added GitHub Actions CI, automated Windows release workflow, and GitHub Pages project website.
