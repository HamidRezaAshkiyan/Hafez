using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Settings;

namespace HafezLibrary.Models
{
    public class UserConfigModel
    {
        public int Id { get; set; }

        #region Notification

        public string NotificationTitleForeground        { get; set; } = string.Empty;
        public string NotificationTitleBackground        { get; set; } = string.Empty;
        public string NotificationTitleFontSize          { get; set; } = string.Empty;
        public string NotificationTitleFontFamily        { get; set; } = string.Empty;
        public string NotificationPanelForeground        { get; set; } = string.Empty;
        public string NotificationPanelBackground        { get; set; } = string.Empty;
        public string NotificationPanelFontSize          { get; set; } = string.Empty;
        public string NotificationPanelFontFamily        { get; set; } = string.Empty;
        public string NotificationPanelHeight            { get; set; } = string.Empty;
        public string NotificationTitleHeight            { get; set; } = string.Empty;
        public string NotificationPanelTitleVisibility   { get; set; } = string.Empty;
        public string NotificationPanelVerticalAlignment { get; set; } = string.Empty;
        public string NotificationPanelMargin            { get; set; } = string.Empty;
        public string NotificationAnimationMode          { get; set; } = string.Empty;
        public string NotificationAnimationSpeed         { get; set; } = string.Empty;

        #endregion

        #region Quran

        public string QuranTitleStrokeColor       { get; set; } = string.Empty;
        public string QuranTitleStrokeSize        { get; set; } = string.Empty;
        public string QuranPanelStrokeColor       { get; set; } = string.Empty;
        public string QuranPanelStrokeSize        { get; set; } = string.Empty;
        public string QuranTitleForeground        { get; set; } = string.Empty;
        public string QuranTitleBackground        { get; set; } = string.Empty;
        public string QuranPanelForeground        { get; set; } = string.Empty;
        public string QuranPanelBackground        { get; set; } = string.Empty;
        public string QuranPanelFontSize          { get; set; } = string.Empty;
        public string QuranTitleFontSize          { get; set; } = string.Empty;
        public string QuranPanelFontFamily        { get; set; } = string.Empty;
        public string QuranTitleFontFamily        { get; set; } = string.Empty;
        public string QuranPanelHeight            { get; set; } = string.Empty;
        public string QuranTitleHeight            { get; set; } = string.Empty;
        public string QuranPanelTitleVisibility   { get; set; } = string.Empty;
        public string QuranPanelVerticalAlignment { get; set; } = string.Empty;
        public string QuranPanelMargin            { get; set; } = string.Empty;
        public string QuranAnimationMode          { get; set; } = string.Empty;
        public string QuranAnimationSpeed         { get; set; } = string.Empty;
        public int    QuranPageNumber             { get; set; }

        #endregion

        #region Mafatih

        public string MafatihTitleStrokeColor       { get; set; } = string.Empty;
        public string MafatihTitleStrokeSize        { get; set; } = string.Empty;
        public string MafatihPanelStrokeColor       { get; set; } = string.Empty;
        public string MafatihPanelStrokeSize        { get; set; } = string.Empty;
        public string MafatihPanelForeground        { get; set; } = string.Empty;
        public string MafatihPanelBackground        { get; set; } = string.Empty;
        public string MafatihTitleForeground        { get; set; } = string.Empty;
        public string MafatihTitleBackground        { get; set; } = string.Empty;
        public string MafatihTitleFontSize          { get; set; } = string.Empty;
        public string MafatihTitleFontFamily        { get; set; } = string.Empty;
        public string MafatihPanelFontSize          { get; set; } = string.Empty;
        public string MafatihPanelFontFamily        { get; set; } = string.Empty;
        public string MafatihPanelHeight            { get; set; } = string.Empty;
        public string MafatihTitleHeight            { get; set; } = string.Empty;
        public string MafatihPanelTitleVisibility   { get; set; } = string.Empty;
        public string MafatihPanelVerticalAlignment { get; set; } = string.Empty;
        public string MafatihPanelMargin            { get; set; } = string.Empty;
        public string MafatihAnimationMode          { get; set; } = string.Empty;
        public string MafatihAnimationSpeed         { get; set; } = string.Empty;

        #endregion

        #region Quran Translation

        public string QuranTranslationTitleForeground      { get; set; } = string.Empty;
        public string QuranTranslationTitleBackground      { get; set; } = string.Empty;
        public string QuranTranslationTitleFontSize        { get; set; } = string.Empty;
        public string QuranTranslationTitleHeight          { get; set; } = string.Empty;
        public string QuranTranslationTitleFontFamily      { get; set; } = string.Empty;
        public string QuranTranslationPanelTitleVisibility { get; set; } = string.Empty;

        public string QuranTranslationPanelForeground { get; set; } = string.Empty;
        public string QuranTranslationPanelBackground { get; set; } = string.Empty;
        public string QuranTranslationPanelHeight     { get; set; } = string.Empty;
        public string QuranTranslationPanelFontSize   { get; set; } = string.Empty;
        public string QuranTranslationPanelFontFamily { get; set; } = string.Empty;

        public string QuranTranslationPanelVerticalAlignment { get; set; } = string.Empty;
        public string QuranTranslationPanelMargin            { get; set; } = string.Empty;
        public string QuranTranslationAnimationMode          { get; set; } = string.Empty;
        public string QuranTranslationAnimationSpeed         { get; set; } = string.Empty;

        #endregion

        #region Mafatih Translation

        public string MafatihTranslationTitleForeground      { get; set; } = string.Empty;
        public string MafatihTranslationTitleBackground      { get; set; } = string.Empty;
        public string MafatihTranslationTitleFontSize        { get; set; } = string.Empty;
        public string MafatihTranslationTitleHeight          { get; set; } = string.Empty;
        public string MafatihTranslationTitleFontFamily      { get; set; } = string.Empty;
        public string MafatihTranslationPanelTitleVisibility { get; set; } = string.Empty;

        public string MafatihTranslationPanelForeground { get; set; } = string.Empty;
        public string MafatihTranslationPanelBackground { get; set; } = string.Empty;
        public string MafatihTranslationPanelHeight     { get; set; } = string.Empty;
        public string MafatihTranslationPanelFontSize   { get; set; } = string.Empty;
        public string MafatihTranslationPanelFontFamily { get; set; } = string.Empty;

        public string MafatihTranslationPanelVerticalAlignment { get; set; } = string.Empty;
        public string MafatihTranslationPanelMargin            { get; set; } = string.Empty;
        public string MafatihTranslationAnimationMode          { get; set; } = string.Empty;
        public string MafatihTranslationAnimationSpeed         { get; set; } = string.Empty;

        #endregion

        #region Other

        public string ControllerMonitorIndex   { get; set; } = string.Empty;
        public string OutputMonitorIndex       { get; set; } = string.Empty;
        public string PanelImageLocation       { get; set; } = string.Empty;
        public string ScreenSaverImageLocation { get; set; } = string.Empty;
        public string CameraShowStatus         { get; set; } = string.Empty;
        public string DefaultCameraName        { get; set; } = string.Empty;
        public string CompanyName              { get; set; } = string.Empty;
        public string ScreenSaver              { get; set; } = string.Empty;
        public string LogoStart                { get; set; } = string.Empty;
        public int    PortNumber               { get; set; } = 5000;
        public string IsListenerAutoStart      { get; set; } = string.Empty;

        #endregion

        public object GetProperty(string propertyName)
        {
            return typeof(UserConfigModel).GetProperty(propertyName)?.GetValue(this);
        }

        public void SetProperty(string propertyName, object value)
        {
            typeof(UserConfigModel).GetProperty(propertyName)?.SetValue(this, value);
        }

        private static readonly HashSet<string> ValidColumns = new(
            typeof(UserConfigModel).GetProperties().Select(p => p.Name),
            StringComparer.OrdinalIgnoreCase);

        public static void Update(Dictionary<string, string> par)
        {
            if (par == null || par.Count == 0)
            {
                return;
            }

            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object>();

            foreach (var kvp in par)
            {
                if (!ValidColumns.Contains(kvp.Key))
                {
                    continue;
                }

                string paramName = $"@p_{kvp.Key}";
                setClauses.Add($"{kvp.Key} = {paramName}");
                parameters[paramName] = kvp.Value;
            }

            if (setClauses.Count == 0)
            {
                return;
            }

            string query = $"UPDATE UserConfig SET {string.Join(", ", setClauses)} WHERE Id = 1";
            DataAccess.Connector.DataAccess.DoCommand(query, parameters);

            // Mirror updates into %AppData%\Hafez\settings.json
            try
            {
                AppSettings settings = AppSettingsService.Load();
                UserConfigModel current = GetAllUserConfig();
                if (current != null)
                {
                    settings.DisplayConfig = current;
                    AppSettingsService.Save(settings);
                }
            }
            catch
            {
                // Non-fatal if JSON settings mirror fails during unit test or read-only env
            }
        }

        public static UserConfigModel GetAllUserConfig()
        {
            const string query = "SELECT * FROM UserConfig WHERE Id = 1";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<UserConfigModel>(query).FirstOrDefault() ?? new UserConfigModel { Id = 1 };
        }
    }
}