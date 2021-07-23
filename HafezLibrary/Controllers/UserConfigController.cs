using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class UserConfigController
    {
        public static List<UserConfigModel> GetAllUserConfigList()
        {
            const string query = "SELECT * FROM UserConfig";

            using IDbConnection   connection = new SqlConnection(SqlConnector.GetConnectionString());
            List<UserConfigModel> output     = connection.Query<UserConfigModel>(query).ToList();

            return output;
        }

        public static void ImportUserConfig(UserConfigModel model)
        {
            const string query = "DELETE FROM UserConfig;" +
                                 "INSERT INTO UserConfig (Id,NotificationTitleFontFamily,NotificationTitleHeight,NotificationTitleFontSize,NotificationTitleForeground,NotificationTitleBackground,NotificationPanelFontSize,NotificationPanelFontFamily,NotificationPanelForeground,NotificationPanelBackground,NotificationPanelHeight,NotificationPanelMargin,NotificationPanelVerticalAlignment,NotificationPanelTitleVisibility,NotificationAnimationMode,NotificationAnimationSpeed,QuranTitleFontFamily,QuranTitleHeight,QuranTitleFontSize,QuranTitleForeground,QuranTitleBackground,QuranPanelFontSize,QuranPanelFontFamily,QuranPanelForeground,QuranPanelBackground,QuranPanelHeight,QuranPanelMargin,QuranPanelVerticalAlignment,QuranPanelTitleVisibility,QuranAnimationMode,QuranAnimationSpeed,QuranPageNumber,MafatihTitleFontFamily,MafatihTitleHeight,MafatihTitleFontSize,MafatihTitleForeground,MafatihTitleBackground,MafatihPanelFontFamily,MafatihPanelFontSize,MafatihPanelForeground,MafatihPanelBackground,MafatihPanelHeight,MafatihPanelMargin,MafatihPanelVerticalAlignment,MafatihPanelTitleVisibility,MafatihAnimationMode,MafatihAnimationSpeed,TranslationTitleForeground,TranslationTitleBackground,TranslationTitleFontFamily,TranslationTitleFontSize,TranslationTitleHeight,TranslationPanelTitleVisibility,TranslationPanelForeground,TranslationPanelBackground,TranslationPanelFontFamily,TranslationPanelFontSize,TranslationPanelHeight,TranslationPanelMargin,TranslationPanelVerticalAlignment,TranslationAnimationMode,TranslationAnimationSpeed,ControllerMonitorIndex,OutputMonitorIndex,ScreenSaverImageLocation,PanelImageLocation,CameraShowStatus,DefaultCameraName,CompanyName,ScreenSaver,LogoStart,PortNumber) " +
                                 "VALUES (@Id,@NotificationTitleFontFamily,@NotificationTitleHeight,@NotificationTitleFontSize,@NotificationTitleForeground,@NotificationTitleBackground,@NotificationPanelFontSize,@NotificationPanelFontFamily,@NotificationPanelForeground,@NotificationPanelBackground,@NotificationPanelHeight,@NotificationPanelMargin,@NotificationPanelVerticalAlignment,@NotificationPanelTitleVisibility,@NotificationAnimationMode,@NotificationAnimationSpeed,@QuranTitleFontFamily,@QuranTitleHeight,@QuranTitleFontSize,@QuranTitleForeground,@QuranTitleBackground,@QuranPanelFontSize,@QuranPanelFontFamily,@QuranPanelForeground,@QuranPanelBackground,@QuranPanelHeight,@QuranPanelMargin,@QuranPanelVerticalAlignment,@QuranPanelTitleVisibility,@QuranAnimationMode,@QuranAnimationSpeed,@QuranPageNumber,@MafatihTitleFontFamily,@MafatihTitleHeight,@MafatihTitleFontSize,@MafatihTitleForeground,@MafatihTitleBackground,@MafatihPanelFontFamily,@MafatihPanelFontSize,@MafatihPanelForeground,@MafatihPanelBackground,@MafatihPanelHeight,@MafatihPanelMargin,@MafatihPanelVerticalAlignment,@MafatihPanelTitleVisibility,@MafatihAnimationMode,@MafatihAnimationSpeed,@TranslationTitleForeground,@TranslationTitleBackground,@TranslationTitleFontFamily,@TranslationTitleFontSize,@TranslationTitleHeight,@TranslationPanelTitleVisibility,@TranslationPanelForeground,@TranslationPanelBackground,@TranslationPanelFontFamily,@TranslationPanelFontSize,@TranslationPanelHeight,@TranslationPanelMargin,@TranslationPanelVerticalAlignment,@TranslationAnimationMode,@TranslationAnimationSpeed,@ControllerMonitorIndex,@OutputMonitorIndex,@ScreenSaverImageLocation,@PanelImageLocation,@CameraShowStatus,@DefaultCameraName,@CompanyName,@ScreenSaver,@LogoStart,@PortNumber);";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, model);
        }
    }
}