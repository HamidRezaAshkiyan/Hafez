using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;
using HafezLibrary.Settings;

namespace HafezLibrary.Controllers
{
    public static class UserConfigController
    {
        public static List<UserConfigModel> GetAllUserConfigList()
        {
            const string query = "SELECT * FROM UserConfig";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<UserConfigModel>(query).ToList();
        }

        public static void ImportUserConfig(UserConfigModel model)
        {
            const string query = "DELETE FROM UserConfig; " +
                                 "INSERT INTO UserConfig (Id,NotificationTitleFontFamily,NotificationTitleHeight,NotificationTitleFontSize,NotificationTitleForeground,NotificationTitleBackground,NotificationPanelFontSize,NotificationPanelFontFamily,NotificationPanelForeground,NotificationPanelBackground,NotificationPanelHeight,NotificationPanelMargin,NotificationPanelVerticalAlignment,NotificationPanelTitleVisibility,NotificationAnimationMode,NotificationAnimationSpeed,QuranTitleFontFamily,QuranTitleHeight,QuranTitleFontSize,QuranTitleForeground,QuranTitleBackground,QuranPanelFontSize,QuranPanelFontFamily,QuranPanelForeground,QuranPanelBackground,QuranPanelHeight,QuranPanelMargin,QuranPanelVerticalAlignment,QuranPanelTitleVisibility,QuranAnimationMode,QuranAnimationSpeed,QuranPageNumber,MafatihTitleFontFamily,MafatihTitleHeight,MafatihTitleFontSize,MafatihTitleForeground,MafatihTitleBackground,MafatihPanelFontFamily,MafatihPanelFontSize,MafatihPanelForeground,MafatihPanelBackground,MafatihPanelHeight,MafatihPanelMargin,MafatihPanelVerticalAlignment,MafatihPanelTitleVisibility,MafatihAnimationMode,MafatihAnimationSpeed,ControllerMonitorIndex,OutputMonitorIndex,ScreenSaverImageLocation,PanelImageLocation,CameraShowStatus,DefaultCameraName,CompanyName,ScreenSaver,LogoStart,PortNumber,IsListenerAutoStart) " +
                                 "VALUES (@Id,@NotificationTitleFontFamily,@NotificationTitleHeight,@NotificationTitleFontSize,@NotificationTitleForeground,@NotificationTitleBackground,@NotificationPanelFontSize,@NotificationPanelFontFamily,@NotificationPanelForeground,@NotificationPanelBackground,@NotificationPanelHeight,@NotificationPanelMargin,@NotificationPanelVerticalAlignment,@NotificationPanelTitleVisibility,@NotificationAnimationMode,@NotificationAnimationSpeed,@QuranTitleFontFamily,@QuranTitleHeight,@QuranTitleFontSize,@QuranTitleForeground,@QuranTitleBackground,@QuranPanelFontSize,@QuranPanelFontFamily,@QuranPanelForeground,@QuranPanelBackground,@QuranPanelHeight,@QuranPanelMargin,@QuranPanelVerticalAlignment,@QuranPanelTitleVisibility,@QuranAnimationMode,@QuranAnimationSpeed,@QuranPageNumber,@MafatihTitleFontFamily,@MafatihTitleHeight,@MafatihTitleFontSize,@MafatihTitleForeground,@MafatihTitleBackground,@MafatihPanelFontFamily,@MafatihPanelFontSize,@MafatihPanelForeground,@MafatihPanelBackground,@MafatihPanelHeight,@MafatihPanelMargin,@MafatihPanelVerticalAlignment,@MafatihPanelTitleVisibility,@MafatihAnimationMode,@MafatihAnimationSpeed,@ControllerMonitorIndex,@OutputMonitorIndex,@ScreenSaverImageLocation,@PanelImageLocation,@CameraShowStatus,@DefaultCameraName,@CompanyName,@ScreenSaver,@LogoStart,@PortNumber,@IsListenerAutoStart);";

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, model);

            AppSettings settings = AppSettingsService.Load();
            settings.DisplayConfig = model;
            AppSettingsService.Save(settings);
        }
    }
}
