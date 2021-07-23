using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using HafezWPFUI.Helper.Handlers;
using Label = System.Windows.Controls.Label;

namespace HafezWPFUI.Views.Windows
{
    [Obfuscation(Exclude = true)]
    public partial class OutputWindowView
    {
        public static StringBuilder OutputStringBuilder { get; } = new();

        #region Notification

        public static ThicknessAnimation NotificationAnimation   { get; set; } = new();
        public static DataTable          NotificationOutputTable { get; set; } = new();
        public static int                NotificationRowCounter  { get; set; } = 0;

        #endregion

        #region Quran

        // public static ThicknessAnimation QuranAnimation { get; set; } = new ThicknessAnimation();
        public static DataTable QuranOutputTable { get; set; } = new();
        public static int       QuranRowCounter  { get; set; }

        #endregion

        #region Mafatih

        // public static ThicknessAnimation MafatihAnimation { get; set; } = new ThicknessAnimation();
        public static DataTable MafatihOutputTable { get; set; } = new();
        public static int       MafatihRowCounter  { get; set; }

        #endregion

        #region Mafatih

        // public static ThicknessAnimation PersonalDuaAnimation { get; set; } = new ThicknessAnimation();
        public static DataTable PersonalDuaOutputTable { get; set; } = new();
        public static int       PersonalDuaRowCounter  { get; set; }

        #endregion

        #region QuranTranslation

        // public static ThicknessAnimation QuranTranslationAnimation { get; set; } = new ThicknessAnimation();
        public static DataTable QuranTranslationOutputTable { get; set; } = new();
        public static int       QuranTranslationRowCounter  { get; set; } = 0;

        #endregion

        #region MafatihTranslation

        // public static ThicknessAnimation MafatihTranslationAnimation { get; set; } = new ThicknessAnimation();
        public static DataTable MafatihTranslationOutputTable { get; set; } = new();
        public static int       MafatihTranslationRowCounter  { get; set; } = 0;

        #endregion

        public object this[string propertyName]
        {
            get => typeof(OutputWindowView).GetProperty(propertyName)?.GetValue(this);
            set => typeof(OutputWindowView).GetProperty(propertyName)?.SetValue(this, value);
        }

        public OutputWindowView()
        {
            InitializeComponent();
        }

        private void OutputWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;

            string defaultImageLocation =
                AppDomain.CurrentDomain.BaseDirectory + @"Gallery\CompanyLogo\";
            Logo.Source = (defaultImageLocation + GlobalConfig.UserConfig.ScreenSaverImageLocation).GetImageByAddress();
            LogoScreenSaver.Source = (defaultImageLocation + GlobalConfig.UserConfig.ScreenSaverImageLocation)
                .GetImageByAddress();
            NotificationPanelLogo.Source =
                (defaultImageLocation + GlobalConfig.UserConfig.PanelImageLocation).GetImageByAddress();

            if ( GlobalConfig.UserConfig.ScreenSaver == "D" )
            {
                StopScreenSaver();
            }
            else
            {
                StartScreenSaver();
            }

            const Enums.PanelTypes notificationPanel = Enums.PanelTypes.Notification;
            ReplaceMarqueeText(notificationPanel.ToString(), GlobalConfig.UserConfig.CompanyName);
            ReplaceOutputTable(notificationPanel, GetPanelMarquee(notificationPanel.ToString())?.Text);

            //Panels Default
            foreach ( Enums.PanelTypes panelType in (Enums.PanelTypes[]) Enum.GetValues(typeof(Enums.PanelTypes)) )
            {
                string panelName = panelType.ToString();

                SetPanelVerticalAlignment(panelName,
                                          $"{panelName}PanelVerticalAlignment".GetProperty().ToString() == "U"
                                              ? VerticalAlignment.Top
                                              : VerticalAlignment.Bottom);

                // Translation Dose not have title
                if ( panelType == Enums.PanelTypes.QuranTranslation ||
                     panelType == Enums.PanelTypes.MafatihTranslation )
                {
                    return;
                }

                TitleBoxVisibilityToggle(panelName,
                                         $"{panelName}PanelTitleVisibility".GetProperty().ToString() == "D"
                                             ? Visibility.Collapsed
                                             : Visibility.Visible);
            }

            // AddOnComplete(notificationPanel.ToString());
        }

        public void ScreenSaverManager()
        {
            if ( GlobalConfig.EnabledServiceCount <= 0 )
            {
                if ( GlobalConfig.UserConfig.ScreenSaver == "E" )
                {
                    LogoScreenSaver.Visibility = Visibility.Visible;
                }
            }
            else
            {
                LogoScreenSaver.Visibility = Visibility.Collapsed;
            }
        }

        public void StartScreenSaver()
        {
            if ( GlobalConfig.UserConfig.ScreenSaver != "E" )
            {
                return;
            }

            if ( GlobalConfig.EnabledServiceCount <= 0 )
            {
                LogoScreenSaver.Visibility = Visibility.Visible;
            }
        }

        public void StopScreenSaver()
        {
            LogoScreenSaver.Visibility = Visibility.Collapsed;
        }

        public void LogoVisibilityToggle(Visibility visibility)
        {
            Storyboard storyboard = (Storyboard) Resources["LogoStoryBoard"];
            if ( visibility == Visibility.Visible )
            {
                storyboard.Duration    = new Duration(new TimeSpan(0, 0, 3));
                storyboard.AutoReverse = true;
                Logo.Visibility        = Visibility.Visible;
            }
            else
            {
                storyboard.Duration = new Duration(new TimeSpan(0, 0, 0));
                Logo.Visibility     = Visibility.Hidden;
            }
        }

        public void TitleBoxVisibilityToggle(string panelName, Visibility visibility)
        {
            Grid titleBox = FindName($"{panelName}TitleBox") as Grid;
            titleBox.Visibility = visibility;
        }

        public void Delay(object sender, EventArgs e)
        {
            Thread.Sleep(10);
            NotificationPanel.Visibility = Visibility.Visible;

            foreach ( Enums.PanelTypes panelType in (Enums.PanelTypes[]) Enum.GetValues(typeof(Enums.PanelTypes)) )
            {
                // Translation Dose not have title
                if ( panelType == Enums.PanelTypes.QuranTranslation ||
                     panelType == Enums.PanelTypes.MafatihTranslation )
                {
                    return;
                }

                TitleBoxVisibilityToggle(panelType.ToString(),
                                         $"{panelType.ToString()}PanelTitleVisibility".GetProperty().ToString() == "D"
                                             ? Visibility.Collapsed
                                             : Visibility.Visible);
            }
        }

        public async void ShowTableInPanel(string panelName, DataTable outputTable)
        {
            // TODO make data table and string same time to change it later
            // TODO Split quran from notification
            if ( outputTable.Rows.Count == 0 )
            {
                MessageBoxHandler.ShowMessage("هشدار", "در این گروه اعلان وجود ندارد");
                return;
            }

            if ( $"{panelName}AnimationMode".GetProperty().ToString() == Enums.AnimationMode.Static.ToString() )
            {
                this[$"{panelName}OutputTable"] = outputTable;
            }
            else
            {
                this[$"{panelName}RowCounter"] = 0;
                ((DataTable) this[$"{panelName}OutputTable"]).Rows.Clear();
                if ( $"{panelName}AnimationMode".GetProperty().ToString() == Enums.AnimationMode.Continuous.ToString() )
                {
                    OutputStringBuilder.Clear();

                    DataTable tempTable = new DataTable();
                    foreach ( DataRow tableRow in outputTable.Rows )
                    {
                        OutputStringBuilder.Append(tableRow["H1"]);
                        OutputStringBuilder.Append("                    ");
                    }

                    tempTable.Columns.Add("H1");


                    tempTable.Rows.Add(OutputStringBuilder.ToString());
                    this[$"{panelName}OutputTable"] = tempTable;
                }
                else
                {
                    this[$"{panelName}OutputTable"] = outputTable;
                }

                StopAnimation(panelName);
                // RemoveOnComplete(panelName);
                SetPanelAnimation(panelName);
                await StartAnimation(panelName);
                // AddOnComplete(panelName);
                //((ThicknessAnimation) this[$"{panelName}Animation"]).Completed += AnimationOnCompleted;

                //AnimationOnCompleted(panelType, null);
            }
        }

        #region Animation

        /*public void SetAnimationMode(PanelTypes panelType, AnimationMode animationMode)
        {
            //TODO set both full table (list) and complete string in class so you can switch easily 
            var panelName = panelType.ToString();

            // check current Mode

            switch (animationMode)
            {
                case AnimationMode.Static:
                    {
                        StopAnimation(panelName);
                    }
                    break;
                case AnimationMode.Continuous:
                    {
                        StopAnimation(panelName);

                        OutputStringBuilder.Clear();
                        var tempTable = new DataTable();
                        var dataTable = this[$"{panelName}OutputTable"] as DataTable;

                        foreach (DataRow tableRow in dataTable.Rows)
                        {
                            OutputStringBuilder.Append(tableRow["H1"]);
                            OutputStringBuilder.Append("                    ");
                        }

                        tempTable.Columns.Add("H1");
                        tempTable.Rows.Add(OutputStringBuilder.ToString());

                        this[$"{panelName}RowCounter"] = 0;
                        ((DataTable)this[$"{panelName}OutputTable"]).Rows.Clear();
                        this[$"{panelName}OutputTable"] = tempTable;

                        StartAnimation(panelName);
                    }
                    break;
                case AnimationMode.Separately:
                    {
                    }
                    break;
            }
        }*/

        public async Task StartAnimation(string panelName)
        {
            if ( panelName == Enums.PanelTypes.QuranTranslation.ToString()
              || panelName == Enums.PanelTypes.MafatihTranslation.ToString()
              || panelName == Enums.PanelTypes.Mafatih.ToString()
              || panelName == Enums.PanelTypes.Quran.ToString() )
            {
                return;
            }

            TextBlock marquee = GetPanelMarquee(panelName);

            AddOnComplete(panelName);
            await Dispatcher.BeginInvoke(new ThreadStart(() => marquee.BeginAnimation(MarginProperty,
                                                             (ThicknessAnimation) this[$"{panelName}Animation"],
                                                             HandoffBehavior.SnapshotAndReplace)));
        }

        public void StopAnimation(string panelName)
        {
            TextBlock marquee = GetPanelMarquee(panelName);

            RemoveOnComplete(panelName);
            marquee.BeginAnimation(MarginProperty, null, HandoffBehavior.SnapshotAndReplace);
        }

        public async void RefreshAnimation(string panelName)
        {
            if ( panelName == Enums.PanelTypes.QuranTranslation.ToString()
              || panelName == Enums.PanelTypes.MafatihTranslation.ToString()
              || panelName == Enums.PanelTypes.Mafatih.ToString()
              || panelName == Enums.PanelTypes.Quran.ToString() )
            {
                return;
            }

            StopAnimation(panelName);
            SetPanelAnimation(panelName);
            await StartAnimation(panelName);
            ((ThicknessAnimation) this[$"{panelName}Animation"]).Completed += AnimationOnCompleted;
        }

        private void SetPanelAnimation(string panelName)
        {
            TextBlock textBlock     = GetPanelMarquee(panelName);
            Size      measureString = MeasureString(panelName, textBlock.Text);

            this[$"{panelName}Animation"] = new ThicknessAnimation
            {
                Name = $"{panelName}Animation",
                From = new Thickness(ActualWidth,          0, 0, 0),
                To   = new Thickness(-measureString.Width, 0, 0, 0),
                Duration = new Duration(
                                        TimeSpan.FromSeconds((measureString.Width + ActualWidth) /
                                                             (Convert.ToDouble($"{panelName}AnimationSpeed"
                                                                                   .GetProperty()) * 10))) // x / t = v 
            };
        }

        private async void AnimationOnCompleted(object sender, EventArgs e)
        {
            ThicknessAnimation animation = ((AnimationClock) sender).Timeline as ThicknessAnimation;
            string             panelName = animation.Name.Substring(0, animation.Name.Length - 9);

            int       rowCounter = (int) this[$"{panelName}RowCounter"];
            DataTable dataTable  = this[$"{panelName}OutputTable"] as DataTable;

            if ( dataTable.Rows.Count == 0 )
            {
                return;
            }

            if ( rowCounter >= dataTable.Rows.Count )
            {
                rowCounter = 0;
            }

            string? nextRowText = dataTable.Rows[rowCounter]["H1"].ToString();

            ToggleMarqueeVisibility(panelName,
                                    Visibility
                                        .Hidden); // because of text jump i hide marquee and then visible after setting animation

            ReplaceMarqueeText(panelName, nextRowText);

            //var animation = SetPanelAnimation();

            //RemoveOnComplete(panelName);
            StopAnimation(panelName);
            SetPanelAnimation(panelName);
            await StartAnimation(panelName);
            // AddOnComplete(panelName);

            ToggleMarqueeVisibility(panelName, Visibility.Visible);

            this[$"{panelName}RowCounter"] = ++rowCounter;
        }

        public void RemoveOnComplete(string panelName)
        {
            ((ThicknessAnimation) this[$"{panelName}Animation"]).Completed -= AnimationOnCompleted;
        }

        public void AddOnComplete(string panelName)
        {
            ((ThicknessAnimation) this[$"{panelName}Animation"]).Completed += AnimationOnCompleted;
        }

        private Size MeasureString(string panelName, string candidate)
        {
            TextBlock textBlock = GetPanelMarquee(panelName);
            FormattedText formattedText = new FormattedText(
                                                            candidate,
                                                            CultureInfo.CurrentCulture,
                                                            0,
                                                            new Typeface(
                                                                         textBlock.FontFamily,
                                                                         textBlock.FontStyle,
                                                                         textBlock.FontWeight,
                                                                         textBlock.FontStretch),
                                                            textBlock.FontSize,
                                                            Brushes.Black,
                                                            new NumberSubstitution(),
                                                            1);

            return new Size(formattedText.Width, formattedText.Height);
        }

        #endregion

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            //this.TryFindParent<MainWindow>().WebCam.CameraControllers("D");
        }

        #region Methods

        #region MonitorSetting

        public void SetPanelMargin(string panelName, int margin)
        {
            StackPanel panel       = FindName($"{panelName}Container") as StackPanel;
            Thickness  panelMargin = new Thickness(0);

            if ( $"{panelName}PanelVerticalAlignment".GetProperty().ToString() == "D" )
            {
                panelMargin.Bottom = margin;
            }
            else
            {
                panelMargin.Top = margin;
            }

            panel.Margin = panelMargin;
        }

        public void SetPanelVerticalAlignment(string panelName, VerticalAlignment alignment)
        {
            WindowState = WindowState.Maximized;

            StackPanel panel = FindName($"{panelName}Container") as StackPanel;

            if ( alignment == VerticalAlignment.Top )
            {
                if ( !BottomStackContainer.Children.Contains(panel) )
                {
                    return;
                }

                BottomStackContainer.Children.Remove(panel);
                TopStackContainer.Children.Add(panel);
            }
            else
            {
                if ( !TopStackContainer.Children.Contains(panel) )
                {
                    return;
                }

                TopStackContainer.Children.Remove(panel);
                BottomStackContainer.Children.Add(panel);
            }

            //SetPanelMargin(panelType, Convert.ToInt32(GlobalConfig.GetField($"{panelType}PanelMargin")));
            //NotificationContainer.VerticalAlignment = alignment;
        }

        public void SetMonitor(int monitorNumber = 1)
        {
            if ( Screen.AllScreens.Length < monitorNumber )
            {
                return;
            }

            WindowState = WindowState.Normal;

            Screen screen = Screen.AllScreens[monitorNumber - 1];
            Left   = screen.Bounds.Left;
            Width  = screen.Bounds.Width;
            Top    = screen.Bounds.Top;
            Height = screen.Bounds.Height;

            WindowState = WindowState.Maximized;
        }

        #endregion

        public void ToggleMarqueeVisibility(string panelName, Visibility visibility)
        {
            TextBlock marquee = GetPanelMarquee(panelName);

            marquee.Visibility = visibility;
        }

        public void ReplaceMarqueeText(string panelName, string text)
        {
            TextBlock marquee = GetPanelMarquee(panelName);

            marquee.Text = text;
        }

        public void ReplaceTitleText(string panelName, string text)
        {
            Label title = FindName($"{panelName}Title") as Label;
            title.Content = text;
        }

        public void ReplaceOutputTable(Enums.PanelTypes panelType, DataTable dataTable)
        {
            //Change header name
            switch ( panelType )
            {
                case Enums.PanelTypes.Notification:
                    {
                        dataTable.Columns["Name"].ColumnName        = "T1";
                        dataTable.Columns["Description"].ColumnName = "H1";
                    }
                    break;
                case Enums.PanelTypes.Quran:
                    {
                        dataTable.Columns["ArabicText"].ColumnName  = "H1";
                        dataTable.Columns["PersianText"].ColumnName = "H2";
                    }
                    break;
                case Enums.PanelTypes.Mafatih:
                    {
                        dataTable.Columns["AyahText"].ColumnName               = "H1";
                        dataTable.Columns["AyahPersianTranslation"].ColumnName = "H2";
                    }
                    break;
                case Enums.PanelTypes.QuranTranslation:
                case Enums.PanelTypes.MafatihTranslation:
                    break;
            }

            //set table
            this[$"{panelType.ToString()}OutputTable"] = dataTable;
        }

        public void ReplaceOutputTable(Enums.PanelTypes panelType, string text)
        {
            string          panelName = panelType.ToString();
            using DataTable tempTable = new DataTable();

            tempTable.Columns.Add("H1");
            tempTable.Columns.Add("H2");
            tempTable.Rows.Add(text);

            this[$"{panelName}OutputTable"] = tempTable;

            // ReplaceMarqueeText(panelName, GlobalConfig.CompanyName);
            // SetPanelAnimation(panelName);
            // ((ThicknessAnimation) this[$"{panelName}Animation"]).Completed += AnimationOnCompleted;
        }

        #endregion

        #region QuranAyahRegion

        public void SetNextAyah()
        {
            if ( QuranRowCounter >= QuranOutputTable.Rows.Count - 1 )
            {
                return;
            }

            QuranRowCounter++;

            ReplaceMarqueeText(Enums.PanelTypes.Quran.ToString(),
                               (string) QuranOutputTable.Rows[QuranRowCounter]["H1"]);
            ReplaceMarqueeText(Enums.PanelTypes.QuranTranslation.ToString(),
                               (string) QuranOutputTable.Rows[QuranRowCounter]["H2"]);

            ReplaceTitleText(Enums.PanelTypes.Quran.ToString(),
                             $"سوره  {QuranOutputTable.Rows[QuranRowCounter]["T1"]} ({QuranOutputTable.Rows[QuranRowCounter]["AyahId"]})");
        }

        public void SetPreviousAyah()
        {
            if ( QuranRowCounter < 1 )
            {
                return;
            }

            QuranRowCounter--;

            ReplaceMarqueeText(Enums.PanelTypes.Quran.ToString(),
                               (string) QuranOutputTable.Rows[QuranRowCounter]["H1"]);
            ReplaceMarqueeText(Enums.PanelTypes.QuranTranslation.ToString(),
                               (string) QuranOutputTable.Rows[QuranRowCounter]["H2"]);

            ReplaceTitleText(Enums.PanelTypes.Quran.ToString(),
                             $"سوره  {QuranOutputTable.Rows[QuranRowCounter]["T1"]} ({QuranOutputTable.Rows[QuranRowCounter]["AyahId"]})");
        }

        public void ResetAyah(int ayahId = 0)
        {
            QuranRowCounter = ayahId;

            ReplaceMarqueeText(Enums.PanelTypes.Quran.ToString(),
                               (string) QuranOutputTable.Rows[QuranRowCounter]["H1"]);
            ReplaceMarqueeText(Enums.PanelTypes.QuranTranslation.ToString(),
                               (string) QuranOutputTable.Rows[QuranRowCounter]["H2"]);

            ReplaceTitleText(Enums.PanelTypes.Quran.ToString(),
                             $"سوره  {QuranOutputTable.Rows[QuranRowCounter]["T1"]} ({QuranOutputTable.Rows[QuranRowCounter]["AyahId"]})");
        }

        public void SetNextFaraz()
        {
            if ( MafatihRowCounter >= MafatihOutputTable.Rows.Count - 1 )
            {
                return;
            }

            MafatihRowCounter++;

            ReplaceMarqueeText(Enums.PanelTypes.Mafatih.ToString(),
                               (string) MafatihOutputTable.Rows[MafatihRowCounter]["H1"]);
            ReplaceMarqueeText(Enums.PanelTypes.MafatihTranslation.ToString(),
                               (string) MafatihOutputTable.Rows[MafatihRowCounter]["H2"]);

            ReplaceTitleText(Enums.PanelTypes.Mafatih.ToString(),
                             $"{MafatihOutputTable.Rows[MafatihRowCounter]["T1"]} ({MafatihOutputTable.Rows[MafatihRowCounter]["FarazId"]})");
        }

        public void SetPreviousFaraz()
        {
            if ( MafatihRowCounter < 1 )
            {
                return;
            }

            MafatihRowCounter--;

            ReplaceMarqueeText(Enums.PanelTypes.Mafatih.ToString(),
                               (string) MafatihOutputTable.Rows[MafatihRowCounter]["H1"]);
            ReplaceMarqueeText(Enums.PanelTypes.MafatihTranslation.ToString(),
                               (string) MafatihOutputTable.Rows[MafatihRowCounter]["H2"]);

            ReplaceTitleText(Enums.PanelTypes.Mafatih.ToString(),
                             $"{MafatihOutputTable.Rows[MafatihRowCounter]["T1"]} ({MafatihOutputTable.Rows[MafatihRowCounter]["FarazId"]})");
        }

        public void ResetFaraz(int farazId = 0)
        {
            MafatihRowCounter = farazId;

            ReplaceMarqueeText(Enums.PanelTypes.Mafatih.ToString(),
                               (string) MafatihOutputTable.Rows[MafatihRowCounter]["H1"]);
            ReplaceMarqueeText(Enums.PanelTypes.MafatihTranslation.ToString(),
                               (string) MafatihOutputTable.Rows[MafatihRowCounter]["H2"]);

            ReplaceTitleText(Enums.PanelTypes.Mafatih.ToString(),
                             $"{MafatihOutputTable.Rows[MafatihRowCounter]["T1"]} ({MafatihOutputTable.Rows[MafatihRowCounter]["FarazId"]})");
        }

        #endregion

        public TextBlock GetPanelMarquee(string panelName)
        {
            return FindName($"{panelName}Marquee") as TextBlock;
        }
    }
}