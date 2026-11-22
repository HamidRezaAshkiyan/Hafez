using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HafezLibrary.Controllers;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;
using HafezLibrary.Settings;
using Xunit;

namespace HafezLibrary.Tests
{
    public class SqliteAndSettingsTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly string _tempDbPath;
        private readonly string _tempSettingsPath;

        public SqliteAndSettingsTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "hafez_tests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
            _tempDbPath = Path.Combine(_tempDir, "hafez_test.db");
            _tempSettingsPath = Path.Combine(_tempDir, "settings.json");

            SqlConnector.OverrideDatabasePathForTesting(_tempDbPath);
            AppSettingsService.OverrideSettingsPathForTesting(_tempSettingsPath);
        }

        public void Dispose()
        {
            SqlConnector.OverrideDatabasePathForTesting(null);
            AppSettingsService.OverrideSettingsPathForTesting(null);
            try
            {
                if (Directory.Exists(_tempDir))
                {
                    Directory.Delete(_tempDir, recursive: true);
                }
            }
            catch
            {
                // Ignore temp cleanup file locks on Windows
            }
        }

        [Fact]
        public void SqliteDatabase_ShouldInitializeAndSeedDefaultAdminAndConfig()
        {
            UserModel loginAttempt = new() { UserId = "admin", Password = "admin" };
            UserModel valid = loginAttempt.IsUserValid();

            Assert.NotNull(valid);
            Assert.Equal("admin", valid.UserId);
            Assert.Equal('S', valid.UserType);

            UserConfigModel config = UserConfigModel.GetAllUserConfig();
            Assert.NotNull(config);
            Assert.Equal(5000, config.PortNumber);
        }

        [Fact]
        public void NotificationAndGroupControllers_CrudShouldWorkInSqlite()
        {
            NotificationGroupController.CreateNotificationGroup(new NotificationGroupModel { Name = "گروه تست" });
            List<NotificationGroupModel> groups = NotificationGroupController.GetAllNotificationGroups();
            NotificationGroupModel createdGroup = groups.FirstOrDefault(g => g.Name == "گروه تست");
            Assert.NotNull(createdGroup);

            NotificationModel created = NotificationController.CreateNotification(new NotificationModel
            {
                GroupId = createdGroup.Id,
                SortId = 1,
                Name = "عنوان تست",
                Description = "متن اعلان تست",
                NotificationType = 'L',
                CreatedBy = 1
            });

            Assert.True(created.Id > 0);
            List<NotificationModel> all = NotificationController.GetAllNotifications();
            Assert.Contains(all, n => n.Id == created.Id && n.Description == "متن اعلان تست");
        }

        [Fact]
        public void PersonalDuaListController_CrudShouldWorkInSqlite()
        {
            PersonalDuaListModel created = PersonalDuaListController.CreatePersonalDuaList(
                new PersonalDuaListModel { DuaName = "دعای تست" });

            Assert.True(created.DuaId > 0);
            Assert.Contains(PersonalDuaListController.GetAllPersonalDuaList(), d => d.DuaId == created.DuaId);

            PersonalDuaListController.RemovePersonalDua(created);
            Assert.DoesNotContain(PersonalDuaListController.GetAllPersonalDuaList(), d => d.DuaId == created.DuaId);
        }

        [Fact]
        public void AppSettingsService_ShouldPersistAndReloadJson()
        {
            AppSettings settings = AppSettingsService.Load();
            settings.DarkMode = true;
            settings.ApiPort = 5151;
            settings.CityName = "مشهد";
            AppSettingsService.Save(settings);

            AppSettings reloaded = AppSettingsService.Load();
            Assert.True(reloaded.DarkMode);
            Assert.Equal(5151, reloaded.ApiPort);
            Assert.Equal("مشهد", reloaded.CityName);
        }

        [Fact]
        public void ExcelConnector_ShouldRoundTripNotificationsWithoutOffice()
        {
            string xlsxPath = Path.Combine(_tempDir, "notifications.xlsx");
            var source = new List<NotificationModel>
            {
                new() { SortId = 1, Name = "عنوان ۱", Description = "متن اول" },
                new() { SortId = 2, Name = "عنوان ۲", Description = "متن دوم" }
            };

            ExcelConnector.ExportFromListToExcel(source, xlsxPath);
            List<NotificationModel> imported = ExcelConnector.ImportFromExcelToList(xlsxPath);

            Assert.Equal(2, imported.Count);
            Assert.Equal("متن اول", imported[0].Description);
            Assert.Equal("عنوان ۲", imported[1].Name);
        }
    }
}
