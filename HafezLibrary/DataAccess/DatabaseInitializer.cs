using System;
using System.IO;
using Dapper;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Security;
using Microsoft.Data.Sqlite;

namespace HafezLibrary.DataAccess
{
    /// <summary>
    /// Ensures the SQLite database exists in a writable location (%AppData%\Hafez\hafez.db),
    /// copies a bundled seed database if present, creates missing tables, and seeds default records.
    /// </summary>
    public static class DatabaseInitializer
    {
        private static readonly object InitLock = new();
        private static string _initializedPath;

        public static void EnsureInitialized()
        {
            string dbPath = SqlConnector.GetDatabasePath();

            lock (InitLock)
            {
                if (string.Equals(_initializedPath, dbPath, StringComparison.OrdinalIgnoreCase) && File.Exists(dbPath))
                {
                    return;
                }

                string directory = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(dbPath))
                {
                    string bundledDb = Path.Combine(AppContext.BaseDirectory, "Data", "hafez.db");
                    if (File.Exists(bundledDb))
                    {
                        File.Copy(bundledDb, dbPath, overwrite: false);
                    }
                }

                using var connection = new SqliteConnection(SqlConnector.GetConnectionString());
                connection.Open();

                connection.Execute("PRAGMA foreign_keys = ON; PRAGMA journal_mode = WAL;");
                CreateSchema(connection);
                SeedDefaults(connection);

                _initializedPath = dbPath;
            }
        }

        public static void ResetInitializationState()
        {
            lock (InitLock)
            {
                _initializedPath = null;
            }
        }

        private static void CreateSchema(SqliteConnection connection)
        {
            const string schemaSql = @"
CREATE TABLE IF NOT EXISTS SureList (
    ID            INTEGER PRIMARY KEY,
    SuraName      TEXT NOT NULL,
    AyahCount     INTEGER NOT NULL,
    NozolSort     INTEGER NOT NULL,
    NozolLocation TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Quran (
    ID                     INTEGER PRIMARY KEY,
    SuraID                 INTEGER NOT NULL,
    AyahID                 INTEGER,
    PageNumber             INTEGER,
    AyahText               TEXT NOT NULL,
    AyahPersianTranslation TEXT NOT NULL,
    AyahEnglishTranslation TEXT,
    FOREIGN KEY (SuraID) REFERENCES SureList(ID)
);

CREATE TABLE IF NOT EXISTS MafatihList (
    DuaId   INTEGER PRIMARY KEY,
    DuaName TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Mafatih (
    Id          INTEGER PRIMARY KEY,
    DuaId       INTEGER NOT NULL,
    FarazId     INTEGER NOT NULL,
    ArabicText  TEXT NOT NULL,
    PersianText TEXT NOT NULL,
    FOREIGN KEY (DuaId) REFERENCES MafatihList(DuaId)
);

CREATE TABLE IF NOT EXISTS PersonalDuaList (
    DuaId   INTEGER PRIMARY KEY AUTOINCREMENT,
    DuaName TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS PersonalDua (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    DuaId       INTEGER NOT NULL,
    FarazId     INTEGER NOT NULL,
    ArabicText  TEXT NOT NULL,
    PersianText TEXT NOT NULL,
    FOREIGN KEY (DuaId) REFERENCES PersonalDuaList(DuaId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Users (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId   TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL,
    Name     TEXT NOT NULL,
    Statue   TEXT NOT NULL DEFAULT 'E',
    UserType TEXT NOT NULL DEFAULT 'U'
);

CREATE TABLE IF NOT EXISTS NotificationGroup (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Name      TEXT NOT NULL UNIQUE,
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Notification (
    Id               INTEGER PRIMARY KEY AUTOINCREMENT,
    GroupId          INTEGER,
    SortId           INTEGER DEFAULT 0,
    Name             TEXT,
    Description      TEXT NOT NULL,
    NotificationType TEXT NOT NULL DEFAULT 'L',
    CreatedAt        TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy        INTEGER,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    FOREIGN KEY (GroupId) REFERENCES NotificationGroup(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS UserConfig (
    Id                                       INTEGER PRIMARY KEY,
    NotificationTitleFontFamily              TEXT,
    NotificationTitleHeight                  INTEGER,
    NotificationTitleFontSize                INTEGER,
    NotificationTitleForeground              TEXT,
    NotificationTitleBackground              TEXT,
    NotificationPanelFontSize                INTEGER,
    NotificationPanelFontFamily              TEXT,
    NotificationPanelForeground              TEXT,
    NotificationPanelBackground              TEXT,
    NotificationPanelHeight                  INTEGER,
    NotificationPanelMargin                  INTEGER,
    NotificationPanelVerticalAlignment       TEXT,
    NotificationPanelTitleVisibility         TEXT,
    NotificationAnimationMode                TEXT,
    NotificationAnimationSpeed               INTEGER,
    QuranTitleFontFamily                     TEXT,
    QuranTitleHeight                         INTEGER,
    QuranTitleFontSize                       INTEGER,
    QuranTitleForeground                     TEXT,
    QuranTitleBackground                     TEXT,
    QuranTitleStrokeColor                    TEXT,
    QuranTitleStrokeSize                     INTEGER,
    QuranPanelFontSize                       INTEGER,
    QuranPanelFontFamily                     TEXT,
    QuranPanelForeground                     TEXT,
    QuranPanelBackground                     TEXT,
    QuranPanelStrokeColor                    TEXT,
    QuranPanelStrokeSize                     INTEGER,
    QuranPanelHeight                         INTEGER,
    QuranPanelMargin                         INTEGER,
    QuranPanelVerticalAlignment              TEXT,
    QuranPanelTitleVisibility                TEXT,
    QuranAnimationMode                       TEXT DEFAULT 'Continuous',
    QuranAnimationSpeed                      TEXT,
    QuranPageNumber                          INTEGER,
    MafatihTitleFontFamily                   TEXT,
    MafatihTitleHeight                       INTEGER,
    MafatihTitleFontSize                     INTEGER,
    MafatihTitleForeground                   TEXT,
    MafatihTitleBackground                   TEXT,
    MafatihTitleStrokeColor                  TEXT,
    MafatihTitleStrokeSize                   INTEGER,
    MafatihPanelFontFamily                   TEXT,
    MafatihPanelFontSize                     INTEGER,
    MafatihPanelForeground                   TEXT,
    MafatihPanelBackground                   TEXT,
    MafatihPanelStrokeColor                  TEXT,
    MafatihPanelStrokeSize                   INTEGER,
    MafatihPanelHeight                       INTEGER,
    MafatihPanelMargin                       INTEGER,
    MafatihPanelVerticalAlignment            TEXT,
    MafatihPanelTitleVisibility              TEXT,
    MafatihAnimationMode                     TEXT,
    MafatihAnimationSpeed                    INTEGER,
    QuranTranslationTitleForeground          TEXT,
    QuranTranslationTitleBackground          TEXT,
    QuranTranslationTitleFontFamily          TEXT,
    QuranTranslationTitleFontSize            INTEGER,
    QuranTranslationTitleHeight              INTEGER,
    QuranTranslationPanelTitleVisibility     TEXT,
    QuranTranslationPanelForeground          TEXT,
    QuranTranslationPanelBackground          TEXT,
    QuranTranslationPanelFontFamily          TEXT,
    QuranTranslationPanelFontSize            INTEGER,
    QuranTranslationPanelHeight              INTEGER,
    QuranTranslationPanelMargin              INTEGER,
    QuranTranslationPanelVerticalAlignment   TEXT,
    QuranTranslationAnimationMode            TEXT,
    QuranTranslationAnimationSpeed           INTEGER,
    MafatihTranslationTitleForeground        TEXT,
    MafatihTranslationTitleBackground        TEXT,
    MafatihTranslationTitleFontFamily        TEXT,
    MafatihTranslationTitleFontSize          INTEGER,
    MafatihTranslationTitleHeight            INTEGER,
    MafatihTranslationPanelTitleVisibility   TEXT,
    MafatihTranslationPanelForeground        TEXT,
    MafatihTranslationPanelBackground        TEXT,
    MafatihTranslationPanelFontFamily        TEXT,
    MafatihTranslationPanelFontSize          INTEGER,
    MafatihTranslationPanelHeight            INTEGER,
    MafatihTranslationPanelMargin            INTEGER,
    MafatihTranslationPanelVerticalAlignment TEXT,
    MafatihTranslationAnimationMode          TEXT,
    MafatihTranslationAnimationSpeed         INTEGER,
    ControllerMonitorIndex                   TEXT,
    OutputMonitorIndex                       TEXT,
    ScreenSaverImageLocation                 TEXT,
    PanelImageLocation                       TEXT,
    CameraShowStatus                         TEXT,
    DefaultCameraName                        TEXT,
    CompanyName                              TEXT,
    ScreenSaver                              TEXT,
    LogoStart                                TEXT,
    PortNumber                               INTEGER DEFAULT 5000,
    IsListenerAutoStart                      TEXT DEFAULT 'D'
);

CREATE INDEX IF NOT EXISTS IX_Quran_SuraID ON Quran(SuraID);
CREATE INDEX IF NOT EXISTS IX_Quran_PageNumber ON Quran(PageNumber);
CREATE INDEX IF NOT EXISTS IX_Mafatih_DuaId ON Mafatih(DuaId);
CREATE INDEX IF NOT EXISTS IX_PersonalDua_DuaId ON PersonalDua(DuaId);
CREATE INDEX IF NOT EXISTS IX_Notification_GroupId ON Notification(GroupId);
";
            connection.Execute(schemaSql);
        }

        private static void SeedDefaults(SqliteConnection connection)
        {
            long userCount = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Users");
            if (userCount == 0)
            {
                connection.Execute(
                    "INSERT INTO Users (UserId, Password, Name, Statue, UserType) VALUES (@UserId, @Password, @Name, 'E', 'S')",
                    new
                    {
                        UserId = "admin",
                        Password = PasswordHasher.Hash("admin"),
                        Name = "مدیر سیستم"
                    });
            }

            long groupCount = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM NotificationGroup");
            if (groupCount == 0)
            {
                connection.Execute("INSERT INTO NotificationGroup (Id, Name) VALUES (1, 'عمومی'), (2, 'اعلانات شبکه');");
            }

            long configCount = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM UserConfig");
            if (configCount == 0)
            {
                connection.Execute(@"
INSERT INTO UserConfig (
    Id,
    NotificationTitleFontFamily, NotificationTitleHeight, NotificationTitleFontSize,
    NotificationTitleForeground, NotificationTitleBackground,
    NotificationPanelFontSize, NotificationPanelFontFamily,
    NotificationPanelForeground, NotificationPanelBackground,
    NotificationPanelHeight, NotificationPanelMargin,
    NotificationPanelVerticalAlignment, NotificationPanelTitleVisibility,
    NotificationAnimationMode, NotificationAnimationSpeed,
    QuranTitleFontFamily, QuranTitleHeight, QuranTitleFontSize,
    QuranTitleForeground, QuranTitleBackground,
    QuranPanelFontSize, QuranPanelFontFamily,
    QuranPanelForeground, QuranPanelBackground,
    QuranPanelHeight, QuranPanelMargin,
    QuranPanelVerticalAlignment, QuranPanelTitleVisibility,
    QuranAnimationMode, QuranAnimationSpeed, QuranPageNumber,
    MafatihTitleFontFamily, MafatihTitleHeight, MafatihTitleFontSize,
    MafatihTitleForeground, MafatihTitleBackground,
    MafatihPanelFontFamily, MafatihPanelFontSize,
    MafatihPanelForeground, MafatihPanelBackground,
    MafatihPanelHeight, MafatihPanelMargin,
    MafatihPanelVerticalAlignment, MafatihPanelTitleVisibility,
    MafatihAnimationMode, MafatihAnimationSpeed,
    QuranTranslationPanelVerticalAlignment, MafatihTranslationPanelVerticalAlignment,
    ControllerMonitorIndex, OutputMonitorIndex,
    ScreenSaverImageLocation, PanelImageLocation,
    CameraShowStatus, DefaultCameraName, CompanyName,
    ScreenSaver, LogoStart, PortNumber, IsListenerAutoStart
) VALUES (
    1,
    'Vazir', 50, 28, '#FFFFFFFF', '#FF1B5E20',
    32, 'Vazir', '#FFFFFFFF', '#CC000000',
    70, 0, 'D', 'E', 'E', 15,
    'Vazir', 50, 28, '#FFFFFFFF', '#FF0D47A1',
    36, 'Vazir', '#FFFFFFFF', '#CC000000',
    80, 0, 'U', 'E', 'Continuous', '15', 1,
    'Vazir', 50, 28, '#FFFFFFFF', '#FF4A148C',
    'Vazir', 34, '#FFFFFFFF', '#CC000000',
    80, 0, 'U', 'E', 'Continuous', 15,
    'D', 'D',
    '0', '0',
    'Hafez.png', 'Hafez.png',
    'D', '', 'حافظ',
    'D', 'D', 5000, 'D'
);");
            }
        }
    }
}
