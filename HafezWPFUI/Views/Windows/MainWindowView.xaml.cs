using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using HafezLibrary.Controllers;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;
using HafezWPFUI.Helper.Handlers;
using HafezWPFUI.Models;
using HafezWPFUI.Properties;
using HafezWPFUI.Views.Notification;
using HafezWPFUI.Views.NotificationGroup;
using HafezWPFUI.Views.User;
using static HafezLibrary.Controllers.DbCache;
using static HafezWPFUI.GlobalConfig;
using Application = System.Windows.Application;
using FlowDirection = System.Windows.FlowDirection;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Timer = System.Timers.Timer;
using UserControl = System.Windows.Controls.UserControl;

namespace HafezWPFUI.Views.Windows
{
    public partial class MainWindowView
    {
        public const           int             LastQuranPageNumber = 604;
        public static readonly SolidColorBrush EnabledColor        = Brushes.White;
        public static readonly SolidColorBrush DisabledColor       = Brushes.DarkRed;
        private const          PackIconKind    PlayIcon            = PackIconKind.Play;
        private const          PackIconKind    PauseIcon           = PackIconKind.Pause;

        public MainWindowView()
        {
            InitializeComponent();

            //ApplicationCustomerName.Text = "HRA";// TinyxConnector.CustomerName;

            // Hook up the Elapsed event for the timer. 
            SlideShowTimer.Interval  =  Convert.ToDouble(TxtSlideShowTime.Text) * 1000;
            SlideShowTimer.Elapsed   += OnTimedEvent;
            SlideShowTimer.AutoReset =  true;
            SlideShowTimer.Enabled   =  false;

            // Hook up the Elapsed event for the timer. 
            LogoShowTimer.Interval  =  Convert.ToDouble(TxtLogoShowTime.Text) * 1000;
            LogoShowTimer.Elapsed   += LogoShowTimerOnElapsed;
            LogoShowTimer.AutoReset =  true;
            LogoShowTimer.Enabled   =  false;
        }

        private void LogoShowTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(
                                   new ThreadStart(() => ButtonNextLogo_OnClick(null, null)));
            Dispatcher.BeginInvoke(
                                   new ThreadStart(() => ButtonPlayLogo_OnClick(null, null)));
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(
                                   new ThreadStart(() => ButtonNextImage_OnClick(null, null)));
            Dispatcher.BeginInvoke(
                                   new ThreadStart(() => ButtonPlayImage_OnClick(null, null)));
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalConfig.Main = this;

                UpdatePersonalDuaComboBoxes();
                ComboBoxMafatihDuaNames.ItemsSource = MafatihLists;
                ComboBoxQuranSure.ItemsSource       = SureLists;

                GetGlobalUserConfig();
                //CameraButton_OnClick(null, null);
                //WebcamHandler.CameraControllers("D");
                Output.Show();

                if ( UserConfig.QuranPageNumber <= LastQuranPageNumber )
                {
                    QuranModel pageNumber = Quran.Where(x => x.PageNumber == UserConfig.QuranPageNumber)
                                                 .OrderBy(x => x.SuraID).First();

                    ComboBoxQuranSure.SelectedIndex = pageNumber.SuraID - 1;
                }

                if ( UserConfig.IsListenerAutoStart != "D" )
                {
                    ListenerButton_OnClick(null, null);
                }

                /*if (GlobalConfig.IsListenerAutoStart == true)
                {
                    ListenerButton_OnClick(null, null);
                }*/
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void UpdatePersonalDuaComboBoxes()
        {
            ComboBoxPersonalDuaNames.Items.Clear();
            foreach ( PersonalDuaListDisplayModel personalDuaList in PersonalDuaListsDisplay )
            {
                ComboBoxPersonalDuaNames.Items.Add(personalDuaList);
            }

            // ComboBoxPersonalDuaNames.ItemsSource = PersonalDuaLists;
        }

        private void TxtBoxLoad(object sender, RoutedEventArgs e)
        {
            LoadTextBox(sender);
        }

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public async void NotificationPlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // await Task.Run(ControlHandlers.PhysicalKeyValidation);

                string panelName = Enums.PanelTypes.Notification.ToString();
                if ( PlayPackIcon.Kind == PackIconKind.MessageBulleted )
                {
                    PlayPackIcon.Foreground = DisabledColor;
                    PlayPackIcon.Kind       = PackIconKind.MessageBulletedOff;

                    // Output.NotificationMarquee.Visibility = Visibility.Visible;

                    // TODO change this and move to method self
                    // TODO change "D" and "E" to cost outside this scope
                    Output.LogoVisibilityToggle(UserConfig.LogoStart == "D" ? Visibility.Hidden : Visibility.Visible);

                    //Output.ShowLogo();  
                    Storyboard sb = Output.Resources["LogoStoryBoard"] as Storyboard;
                    sb.Begin(Output.Logo);

                    // Output.Show();
                    Output.NotificationContainer.Visibility = Visibility.Visible;
                    await Output.StartAnimation(panelName);

                    EnabledServiceCount++;
                    Output.StopScreenSaver();
                }
                else
                {
                    PlayPackIcon.Foreground = EnabledColor;
                    PlayPackIcon.Kind       = PackIconKind.MessageBulleted;

                    Output.NotificationContainer.Visibility = Visibility.Collapsed;
                    Output.StopAnimation(panelName);

                    EnabledServiceCount--;
                    Output.StartScreenSaver();

                    await Task.Run(ControlHandlers.PhysicalKeyValidation);
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void CameraButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ControlHandlers.PhysicalKeyValidation();

                if ( WebcamHandler.VideoSource == null )
                {
                    WebcamHandler.GetCamList();
                    WebcamHandler.ComboBoxDefaultCameraName.SelectedItem = UserConfig.DefaultCameraName;
                }

                if ( WebcamHandler.ComboBoxDefaultCameraName.SelectedIndex != -1 )
                {
                    //if (RadWebCam.GetVideoCaptureDevices().Count >= 1)
                    //{
                    string value = CameraPackIcon.Foreground == DisabledColor ? "D" : "E";
                    //Output.PictureInput.Visibility = Visibility.Hidden;
                    WebcamHandler.CameraControllers(value);
                    //}
                }
                else
                {
                    MessageBoxHandler.ShowMessage("هشدار", "دوربینی برای نمایش انتخاب نشده");
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            ClearPage();
            AboutUs.Visibility = Visibility.Visible;
        }

        public void SetMonitor(int monitorNumber = 1)
        {
            if ( Screen.AllScreens.Length < monitorNumber )
            {
                return;
            }

            Screen screen = Screen.AllScreens[monitorNumber - 1];

            Left = screen.Bounds.Left + ((screen.Bounds.Width  - Width)  / 2);
            Top  = screen.Bounds.Top  + ((screen.Bounds.Height - Height) / 2);

            WindowStartupLocation = WindowStartupLocation.Manual;
            WindowState           = WindowState.Normal;
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void ClearPage()
        {
            IEnumerable<DependencyObject> childObjects = Main.GetChildObjects();
            foreach ( DependencyObject dependencyObject in childObjects )
            {
                ((UserControl) dependencyObject).Visibility = Visibility.Collapsed;
            }
        }

        #region SubRegion

        private void Sub1_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            ColorHandler.Visibility = Visibility.Visible;
        }

        private void Sub2_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            SizeHandler.Visibility = Visibility.Visible;
        }

        private void Sub3_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            //MonitorPage.FillMonitorComboBox();
            MonitorHandler.Visibility = Visibility.Visible;
        }

        private void Sub4_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            LoadFileHandler.Visibility = Visibility.Visible;
        }

        private void Sub5_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            WebcamHandler.Visibility = Visibility.Visible;

            WebcamHandler.GetCamList();
            WebcamHandler.ComboBoxDefaultCameraName.SelectedItem = UserConfig.DefaultCameraName;
        }

        private async void Sub6_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            NotificationList.Visibility                       = Visibility.Visible;
            NotificationList.SelectedNotificationGroup        = null;
            NotificationList.NotificationListView.ItemsSource = await NotificationList.GetNotificationListByType();
        }

        private void Sub7_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            NotificationGroupList.Visibility = Visibility.Visible;
        }

        private void Sub8_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            SettingHandler.Visibility = Visibility.Visible;
        }

        private void Sub9_Click(object sender, RoutedEventArgs e)
        {
            ClearPage();
            UserList.Visibility = Visibility.Visible;
        }

        #endregion

        private void ListenerButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( ListenerPackIcon.Kind == PackIconKind.Cast )
                {
                    Listener.Show();
                    Listener.StartReceiving();

                    ListenerPackIcon.Kind       = PackIconKind.CastOff;
                    ListenerPackIcon.Foreground = DisabledColor;
                }
                else
                {
                    Listener.Hide();
                    Listener.StopServer();

                    ListenerPackIcon.Kind       = PackIconKind.Cast;
                    ListenerPackIcon.Foreground = EnabledColor;
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void Book_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ControlHandlers.PhysicalKeyValidation();
                // await Task.Run(ControlHandlers.PhysicalKeyValidation);

                if ( PackIconBook.Kind == PackIconKind.BookOpenVariant )
                {
                    Storyboard sb = Resources["SideMenuOpenQuran"] as Storyboard;
                    sb.Begin(SideMenuQuran);

                    PackIconBook.Foreground = DisabledColor;
                    PackIconBook.Kind       = PackIconKind.BookOpenPageVariant;
                }
                else
                {
                    Storyboard sb = Resources["SideMenuCloseQuran"] as Storyboard;
                    sb.Begin(SideMenuQuran);

                    PackIconBook.Foreground = EnabledColor;
                    PackIconBook.Kind       = PackIconKind.BookOpenVariant;
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        #region StackPanelAnimation

        private void StackPanel_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard sb = Resources["SideMenuOpen"] as Storyboard;
            sb.Begin(SideMenu);
        }

        private void StackPanel_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard sb = Resources["SideMenuClose"] as Storyboard;
            sb.Begin(SideMenu);
            //ClearSubMenu();
        }

        #endregion

        #region Quran Mafatih Drawer

        /*private void PageSearch_Click(object sender, RoutedEventArgs e)
        {
            //Check Number input
            if (string.IsNullOrEmpty(TxtQuranPageNumber.Text)) return;
            if (Convert.ToInt32(TxtQuranPageNumber.Text) > LastQuranPageNumber) return;

            //Update DB
            UserConfig.Update(new Dictionary<string, string> { { "QuranPageNumber", TxtQuranPageNumber.Text } });

            //Update GlobalConfig
            GlobalConfig.QuranPageNumber = Convert.ToInt32(TxtQuranPageNumber.Text);

            var dataTablePageNumber = QuranController.GetSureIdByPageNumber(GlobalConfig.QuranPageNumber);
            ComboBoxQuranSure.SelectedIndex = Convert.ToInt32(dataTablePageNumber.Rows[0]["SuraID"]) - 1;
        }*/

        private void TxtQuranPageNumber_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check Number input
                /*if (!TxtQuranPageNumber.IsKeyboardFocused)
            {
                //not to change other while not focused.
                return;
            }*/

                if ( string.IsNullOrWhiteSpace(TxtQuranPageNumber.Text) )
                {
                    e.Handled = true;
                    return;
                }

                int quranPageNumber = Convert.ToInt32(TxtQuranPageNumber.Text);
                if ( quranPageNumber > LastQuranPageNumber || quranPageNumber <= 0 )
                {
                    e.Handled = true;
                    return;
                }

                //Update DB
                UserConfigModel.Update(new Dictionary<string, string>
                {
                    {"QuranPageNumber", quranPageNumber.ToString()}
                });

                //Update GlobalConfig
                UserConfig.QuranPageNumber = quranPageNumber;

                /*var dataTablePageNumber = QuranController.GetSureIdByPageNumber(GlobalConfig.QuranPageNumber);
                ComboBoxQuranSure.SelectedIndex = Convert.ToInt32(dataTablePageNumber.Rows[0]["SuraID"]) - 1;*/

                List<QuranModel> selectedQuran = Quran.Where(q => q.PageNumber == quranPageNumber).ToList();

                // Sure ComboBox
                ComboBoxQuranSure.SelectedIndex = Convert.ToInt32(selectedQuran.First().SuraID) - 1;

                // Ayah ComboBox
                ComboBoxQuranAyah.Items.Clear();
                foreach ( QuranModel q in selectedQuran )
                {
                    ComboBoxQuranAyah.Items.Add(q);
                }

                // Ayah Count
                TextBlockAyahCount.Text = $"{selectedQuran.Count} آیه";

                // Set Output
                DataTable outputTable = selectedQuran.ToDataTable();

                outputTable.Columns["SuraName"].ColumnName               = "T1";
                outputTable.Columns["AyahText"].ColumnName               = "H1";
                outputTable.Columns["AyahPersianTranslation"].ColumnName = "H2";

                Output.ShowTableInPanel(Enums.PanelTypes.Quran.ToString(), outputTable);
                //Output.ReplaceTitleText(PanelTypes.Quran.ToString(), selectedQuran.First().SuraName);
                Output.ResetAyah();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxQuranSure_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                /*if (!ComboBoxQuranSure.IsKeyboardFocused)
                {
                    //not to change other while not focused.
                    return;
                }*/

                SureListModel    selectedSure  = ComboBoxQuranSure.SelectedItem as SureListModel;
                List<QuranModel> selectedQuran = Quran.Where(q => q.SuraID == selectedSure.Id).ToList();

                // PageNumber TextBox
                //TxtQuranPageNumber.Text = selectedQuran.First().PageNumber.ToString();

                // Ayah ComboBox
                ComboBoxQuranAyah.Items.Clear();
                foreach ( QuranModel q in selectedQuran )
                {
                    ComboBoxQuranAyah.Items.Add(q);
                }

                // Ayah Count
                TextBlockAyahCount.Text = $"{selectedQuran.Count} آیه";

                // Set Output
                DataTable outputTable = selectedQuran.ToDataTable();

                outputTable.Columns["SuraName"].ColumnName               = "T1";
                outputTable.Columns["AyahText"].ColumnName               = "H1";
                outputTable.Columns["AyahPersianTranslation"].ColumnName = "H2";

                Output.ShowTableInPanel(Enums.PanelTypes.Quran.ToString(), outputTable);
                //Output.ReplaceTitleText(PanelTypes.Quran.ToString(), selectedQuran.First().SuraName);
                Output.ResetAyah();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxQuranAyah_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                /*if (!ComboBoxQuranAyah.IsKeyboardFocused)
                {
                    //not to change other while not focused.
                    return;
                }*/

                if ( !(ComboBoxQuranAyah.SelectedItem is QuranModel) )
                {
                    return;
                }

                // PageNumber TextBox
                //TxtQuranPageNumber.Text = selectedAyah.PageNumber.ToString();

                // Sura ComboBox
                //ComboBoxQuranSure.SelectedIndex = Convert.ToInt32(selectedAyah.SuraID) - 1;

                // Set Output
                Output.ResetAyah(ComboBoxQuranAyah.SelectedIndex);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxMafatihDuaNames_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MafatihListModel mafatihList = (MafatihListModel) ComboBoxMafatihDuaNames.SelectedItem;
                DataTable        farazes     = MafatihController.GetFarazTableByDuaId_Obsolete(mafatihList.DuaId);

                farazes.Columns["DuaName"].ColumnName     = "T1";
                farazes.Columns["ArabicText"].ColumnName  = "H1";
                farazes.Columns["PersianText"].ColumnName = "H2";

                Output.ShowTableInPanel(Enums.PanelTypes.Mafatih.ToString(), farazes);
                Output.ReplaceTitleText(Enums.PanelTypes.Mafatih.ToString(), mafatihList.DuaName);

                Output.ResetFaraz();

                ComboBoxMafatihFarazId.Items.Clear();
                foreach ( DataRow faraz in farazes.Rows )
                {
                    ComboBoxMafatihFarazId.Items.Add(faraz["FarazId"]);
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void QuranPlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( string.IsNullOrWhiteSpace(TxtQuranPageNumber.Text) )
                {
                    return;
                }

                // const PackIconKind playIcon = PackIconKind.Play;
                // const PackIconKind pauseIcon = PackIconKind.Pause;

                if ( QuranPlayPackIcon.Kind == PlayIcon )
                {
                    QuranPlayPackIcon.Kind = PauseIcon;

                    //var sureList = (SureListModel) ComboBoxQuranSure.SelectedItem;
                    //var ayahs = QuranController.GetAyahsByPageNumber(Convert.ToInt32(TxtQuranPageNumber.Text));

                    // if ( UserConfig.QuranAnimationMode == AnimationMode.Static.ToString() )
                    // {
                    Output.QuranMarquee.HorizontalAlignment = HorizontalAlignment.Center;
                    Output.QuranMarquee.TextAlignment       = TextAlignment.Center;
                    Output.QuranMarquee.TextWrapping        = TextWrapping.Wrap;
                    Output.QuranMarquee.FlowDirection       = FlowDirection.RightToLeft;

                    Output.QuranTranslationMarquee.TextAlignment = TextAlignment.Center;
                    // }
                    // else
                    // {
                    //     Output.QuranMarquee.HorizontalAlignment = HorizontalAlignment.Left;
                    //     Output.QuranMarquee.TextWrapping = TextWrapping.NoWrap;
                    //     Output.QuranMarquee.FlowDirection = FlowDirection.LeftToRight;
                    //     await Output.StartAnimation(PanelTypes.Quran.ToString());
                    // }

                    /*Output.ReplaceTitleText(PanelTypes.Quran.ToString(),
                    sureList.SureName);
                    Output.ShowTableInPanel(PanelTypes.Quran.ToString(), ayahs);*/

                    // Output.Show();
                    Output.QuranContainer.Visibility            = Visibility.Visible;
                    Output.QuranTranslationContainer.Visibility = Visibility.Visible;


                    EnabledServiceCount++;
                    Output.StopScreenSaver();
                }
                else
                {
                    QuranPlayPackIcon.Kind = PlayIcon;

                    Output.QuranContainer.Visibility            = Visibility.Collapsed;
                    Output.QuranTranslationContainer.Visibility = Visibility.Collapsed;

                    // Output.StopAnimation(PanelTypes.Quran.ToString());

                    EnabledServiceCount--;
                    Output.StartScreenSaver();
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonNextQuranAyah_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Output.SetNextAyah();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonPreviousQuranAyah_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Output.SetPreviousAyah();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void ButtonStopQuran_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Output.ResetAyah();
                Output.StopScreenSaver();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxMafatihFarazId_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ( ComboBoxMafatihFarazId.SelectedIndex == -1 )
                {
                    return;
                }

                // Set Output
                Output.ResetFaraz(ComboBoxMafatihFarazId.SelectedIndex);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void MafatihPlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( ComboBoxMafatihDuaNames.SelectedIndex == -1 )
                {
                    return;
                }

                string panelName = Enums.PanelTypes.Mafatih.ToString();

                if ( MafatihPlayPackIcon.Kind == PlayIcon )
                {
                    MafatihPlayPackIcon.Kind = PauseIcon;

                    MafatihListModel selectedMafatihList = (MafatihListModel) ComboBoxMafatihDuaNames.SelectedItem;
                    int              duaId               = selectedMafatihList.DuaId;
                    DataTable        farazes             = MafatihController.GetFarazTableByDuaId_Obsolete(duaId);

                    // if ( UserConfig.MafatihAnimationMode == AnimationMode.Static.ToString() )
                    // {
                    Output.MafatihMarquee.HorizontalAlignment = HorizontalAlignment.Center;
                    Output.MafatihMarquee.TextAlignment       = TextAlignment.Center;
                    Output.MafatihMarquee.TextWrapping        = TextWrapping.Wrap;
                    Output.MafatihMarquee.FlowDirection       = FlowDirection.RightToLeft;

                    Output.MafatihTranslationMarquee.TextAlignment = TextAlignment.Center;
                    // }
                    // else
                    // {
                    //     Output.MafatihMarquee.HorizontalAlignment = HorizontalAlignment.Left;
                    //     Output.MafatihMarquee.TextWrapping = TextWrapping.NoWrap;
                    //     Output.MafatihMarquee.FlowDirection = FlowDirection.LeftToRight;
                    //     await Output.StartAnimation(panelName);
                    // }

                    farazes.Columns["DuaName"].ColumnName     = "T1";
                    farazes.Columns["ArabicText"].ColumnName  = "H1";
                    farazes.Columns["PersianText"].ColumnName = "H2";

                    Output.ShowTableInPanel(panelName, farazes);
                    Output.ReplaceTitleText(panelName,
                                            $"{selectedMafatihList.DuaName} ({OutputWindowView.MafatihOutputTable.Rows[OutputWindowView.MafatihRowCounter]["FarazId"]})");

                    // Output.Show();
                    Output.MafatihContainer.Visibility            = Visibility.Visible;
                    Output.MafatihTranslationContainer.Visibility = Visibility.Visible;


                    EnabledServiceCount++;
                    Output.StopScreenSaver();
                }
                else
                {
                    MafatihPlayPackIcon.Kind = PlayIcon;

                    Output.MafatihContainer.Visibility            = Visibility.Collapsed;
                    Output.MafatihTranslationContainer.Visibility = Visibility.Collapsed;

                    // Output.StopAnimation(panelName);


                    EnabledServiceCount--;
                    Output.StartScreenSaver();
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonMafatihNextFaraz_OnClick(object sender, RoutedEventArgs e)
        {
            Output.SetNextFaraz();
        }

        private void ButtonMafatihPreviousFaraz_OnClick(object sender, RoutedEventArgs e)
        {
            Output.SetPreviousFaraz();
        }

        private void ButtonMafatihRestartFaraz_OnClick(object sender, RoutedEventArgs e)
        {
            Output.ResetFaraz();
        }

        #endregion

        #region Picture Drawer

        private int          CurrentImageIndex { get; set; }
        private List<string> ImageNames        { get; set; } = new();
        private Timer        SlideShowTimer    { get; }      = new();

        private int          CurrentLogoIndex { get; set; }
        private List<string> LogoNames        { get; set; } = new();
        private Timer        LogoShowTimer    { get; }      = new();

        public void ImageSlide_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ControlHandlers.PhysicalKeyValidation();
                // await Task.Run(ControlHandlers.PhysicalKeyValidation);

                if ( PackIconSlide.Kind == PackIconKind.ImageSizeSelectActual )
                {
                    Storyboard sb = Resources["SideMenuOpenSlideImage"] as Storyboard;
                    sb?.Begin(SideMenuSlideImage);

                    FillComboboxPicture();
                    FillComboboxLogo();
                    ImgPreview.Source   = null;
                    LblFileName.Content = null;

                    PackIconSlide.Foreground = DisabledColor;
                    PackIconSlide.Kind       = PackIconKind.ImageOutline;
                }
                else
                {
                    Storyboard sb = Resources["SideMenuCloseSlideImage"] as Storyboard;
                    sb?.Begin(SideMenuSlideImage);

                    PackIconSlide.Foreground = EnabledColor;
                    PackIconSlide.Kind       = PackIconKind.ImageSizeSelectActual;
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void FillComboboxPicture()
        {
            List<string> fileNames = new DirectoryInfo(ImageLocation).GetDirectories().Select(o => o.Name).ToList();

            //ComboBoxPicture.ItemsSource = filesName;
            ComboBoxImage.Items.Clear();
            for ( int i = 0; i < fileNames.Count; i++ )
            {
                //var folderName = new ImageFolderName
                //{
                //    Id = i + 1,
                //    FileName = fileNames[i]
                //};
                var folderName = new {Id = i + 1, FileName = fileNames[i]};

                ComboBoxImage.Items.Add(folderName);
            }

            /*ComboBoxFilm.Items.Clear();
            for (int i = 0; i < filmNames.Count; i++)
            {
                var folderName = new
                {
                    Id = i + 1,
                    FileName = filmNames[i]
                };

                ComboBoxFilm.Items.Add(folderName);
            }*/
        }

        public void FillComboboxLogo()
        {
            List<string> fileNames = new DirectoryInfo(LogoLocation).GetDirectories().Select(o => o.Name).ToList();

            ComboBoxLogo.Items.Clear();
            for ( int i = 0; i < fileNames.Count; i++ )
            {
                var folderName = new {Id = i + 1, FileName = fileNames[i]};

                ComboBoxLogo.Items.Add(folderName);
            }
        }

        /*private class ImageFolderName
        {
            public int Id { get; set; }
            public string FileName { get; set; }
        }*/

        private void ComboBoxImage_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ( ComboBoxImage.SelectedItem == null )
                {
                    return;
                }

                dynamic selectedItem = ComboBoxImage.SelectedItem;

                CurrentImageIndex = 0;
                ImageNames.Clear();
                ImageNames = ((string) selectedItem.FileName).GetGalleryFullPath();

                if ( ImageNames.Count == 0 )
                {
                    LblFileName.Content = "";
                    ImgPreview.Source   = null;
                    MessageBoxHandler.ShowMessage("هشدار", "تصویری در این پوشه موجود نیست");
                    return;
                }

                /*if (fileCount != 0)
                {*/

                ImgPreview.Source   = new BitmapImage(new Uri(ImageNames.First()));
                LblFileName.Content = ImageNames.First().Split('\\').ToList().Last();

                /*}
                else
                {
                    Output.PictureInput.Visibility = Visibility.Hidden;
                }*/

                SlideShowImage.IsChecked = false;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxLogo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ( ComboBoxLogo.SelectedItem == null )
                {
                    return;
                }

                dynamic selectedItem = ComboBoxLogo.SelectedItem;

                CurrentLogoIndex = 0;
                LogoNames.Clear();
                LogoNames = ((string) selectedItem.FileName).GetLogoFullPath();

                if ( LogoNames.Count == 0 )
                {
                    LblLogoName.Content = "";
                    LogoPreview.Source  = null;
                    MessageBoxHandler.ShowMessage("هشدار", "تصویری در این پوشه موجود نیست");
                    return;
                }

                /*if (fileCount != 0)
                {*/

                LogoPreview.Source  = new BitmapImage(new Uri(LogoNames.First()));
                LblLogoName.Content = LogoNames.First().Split('\\').ToList().Last();

                /*}
                else
                {
                    Output.PictureInput.Visibility = Visibility.Hidden;
                }*/

                SlideShowLogo.IsChecked = false;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void ButtonPlayImage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( ComboBoxImage.SelectedIndex == -1 )
                {
                    return;
                }

                if ( TxtSlideShowTime.IsEnabled )
                {
                    SlideShowTimer.Start();
                }

                //var selected = ComboBoxPicture.SelectedItem.ToString();
                //var filePaths = GetGalleryFullPath(selected);

                if ( ImageNames.Count == 0 )
                {
                    MessageBoxHandler.ShowMessage("هشدار", "تصویری در این پوشه موجود نیست");
                    return;
                }

                if ( Output.PictureInput.Visibility != Visibility.Visible )
                {
                    EnabledServiceCount++;
                    Output.StopScreenSaver();

                    Output.PictureInput.Visibility = Visibility.Visible;
                }

                ShowImageInOutPutWindow(ImageNames[CurrentImageIndex]);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void ButtonStopImage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( Output.PictureInput.Visibility != Visibility.Collapsed )
                {
                    EnabledServiceCount--;
                    Output.StartScreenSaver();

                    //SlideShowTimer.Enabled = false;
                    SlideShowTimer.Stop();
                    Output.PictureInput.Visibility = Visibility.Collapsed;
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public async void ButtonNextImage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (ComboBoxPicture.SelectedIndex == -1) return;
                int fileCount = ImageNames.Count;

                if ( fileCount != 0 )
                {
                    if ( CurrentImageIndex >= fileCount - 1 )
                    {
                        CurrentImageIndex = -1;
                    }

                    ++CurrentImageIndex;
                    await Dispatcher?.BeginInvoke(new ThreadStart(() =>
                                                                      ImgPreview.Source =
                                                                          new BitmapImage(new Uri(ImageNames
                                                                              [CurrentImageIndex]))));
                    await Dispatcher?.BeginInvoke(new ThreadStart(() =>
                                                                      LblFileName.Content =
                                                                          ImageNames[CurrentImageIndex].Split('\\')
                                                                              .ToList().Last()));

                    //ImgPreview.Source = new BitmapImage(new Uri(ImageNames[CurrentImageIndex]));
                    //LblFileName.Content = Task.Run(() => ImageNames[CurrentImageIndex].Split('\\').ToList().Last());
                }
                else
                {
                    Output.PictureInput.Visibility = Visibility.Hidden;
                    Output.StopScreenSaver();
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void ButtonPreviousImage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( ComboBoxImage.SelectedIndex == -1 )
                {
                    return;
                }

                int fileCount = ImageNames.Count;

                if ( fileCount != 0 )
                {
                    if ( CurrentImageIndex < 1 )
                    {
                        CurrentImageIndex = fileCount;
                    }

                    --CurrentImageIndex;
                    ImgPreview.Source   = new BitmapImage(new Uri(ImageNames[CurrentImageIndex]));
                    LblFileName.Content = ImageNames[CurrentImageIndex].Split('\\').ToList().Last();
                }
                else
                {
                    Output.PictureInput.Visibility = Visibility.Hidden;
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public static void ShowImageInOutPutWindow(string fileName)
        {
            Output.PictureInput.Source = new BitmapImage(new Uri(fileName));
        }

        #endregion

        private void ButtonImageName_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( ImageNames.Count == 0 )
                {
                    return;
                }

                string folderPath = ImageNames[CurrentImageIndex];
                folderPath = folderPath.Remove(folderPath.LastIndexOf("\\", StringComparison.Ordinal));

                if ( ImageNames.Count != 0 ) //&& File.Exists(folderPath))
                {
                    Process.Start("explorer.exe ", folderPath);
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void ButtonPlayLogo_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( ComboBoxLogo.SelectedIndex == -1 )
                {
                    return;
                }

                if ( TxtLogoShowTime.IsEnabled )
                {
                    LogoShowTimer.Start();
                }

                //var selected = ComboBoxPicture.SelectedItem.ToString();
                //var filePaths = GetGalleryFullPath(selected);

                if ( LogoNames.Count == 0 )
                {
                    MessageBoxHandler.ShowMessage("هشدار", "تصویری در این پوشه موجود نیست");
                    return;
                }

                if ( Output.LogoLayer.Visibility != Visibility.Visible )
                {
                    EnabledServiceCount++;
                    Output.StopScreenSaver();

                    Output.LogoLayer.Visibility = Visibility.Visible;
                }

                ShowLogoInOutPutWindow(LogoNames[CurrentLogoIndex]);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void ButtonStopLogo_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( Output.LogoLayer.Visibility != Visibility.Collapsed )
                {
                    EnabledServiceCount--;
                    Output.StartScreenSaver();

                    //SlideShowTimer.Enabled = false;
                    LogoShowTimer.Stop();
                    Output.LogoLayer.Visibility = Visibility.Collapsed;
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public async void ButtonNextLogo_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (ComboBoxPicture.SelectedIndex == -1) return;
                int fileCount = LogoNames.Count;

                if ( fileCount != 0 )
                {
                    if ( CurrentLogoIndex >= fileCount - 1 )
                    {
                        CurrentLogoIndex = -1;
                    }

                    ++CurrentLogoIndex;
                    await Dispatcher?.BeginInvoke(new ThreadStart(() =>
                                                                      LogoPreview.Source =
                                                                          new BitmapImage(new Uri(LogoNames
                                                                              [CurrentLogoIndex]))));
                    await Dispatcher?.BeginInvoke(new ThreadStart(() =>
                                                                      LblLogoName.Content = LogoNames[CurrentLogoIndex]
                                                                          .Split('\\').ToList().Last()));

                    //ImgPreview.Source = new BitmapImage(new Uri(ImageNames[CurrentImageIndex]));
                    //LblFileName.Content = Task.Run(() => ImageNames[CurrentImageIndex].Split('\\').ToList().Last());
                }
                else
                {
                    Output.LogoLayer.Visibility = Visibility.Hidden;
                    Output.StopScreenSaver();
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void ButtonPreviousLogo_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( ComboBoxLogo.SelectedIndex == -1 )
                {
                    return;
                }

                int fileCount = LogoNames.Count;

                if ( fileCount != 0 )
                {
                    if ( CurrentLogoIndex < 1 )
                    {
                        CurrentLogoIndex = fileCount;
                    }

                    --CurrentLogoIndex;
                    LogoPreview.Source  = new BitmapImage(new Uri(LogoNames[CurrentLogoIndex]));
                    LblLogoName.Content = LogoNames[CurrentLogoIndex].Split('\\').ToList().Last();
                }
                else
                {
                    Output.LogoLayer.Visibility = Visibility.Hidden;
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public static void ShowLogoInOutPutWindow(string fileName)
        {
            Output.LogoLayer.Source = new BitmapImage(new Uri(fileName));
        }

        private void ButtonLogoName_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( LogoNames.Count == 0 )
                {
                    return;
                }

                string folderPath = LogoNames[CurrentLogoIndex];
                folderPath = folderPath.Remove(folderPath.LastIndexOf("\\", StringComparison.Ordinal));

                if ( LogoNames.Count != 0 ) //&& File.Exists(folderPath))
                {
                    Process.Start("explorer.exe ", folderPath);
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void SnackbarMessage_OnActionClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SnackbarMessage.IsActive = false;

                ClearPage();
                NotificationList.Visibility            = Visibility.Visible;
                NotificationList.NetTabItem.IsSelected = true;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        #region FilmMethods

        // private List<string> FolderFilmNames { get; set; } = new List<string>();
        private string FilmName { get; set; }

        private void ButtonNextFilm_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using OpenFileDialog ofd = new OpenFileDialog();
                if ( ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK )
                {
                    return;
                }

                FilmName = ofd.FileName;

                SelectedFilmPreview.Source = new Uri(FilmName);
                SelectedFilmPreview.Play();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonStopFilm_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedFilmPreview.Stop();
                Output.SelectedFilmPreview.Stop();

                if ( Output.SelectedFilmPreview.Visibility != Visibility.Collapsed )
                {
                    EnabledServiceCount--;
                    Output.StartScreenSaver();
                }

                Output.SelectedFilmPreview.Visibility = Visibility.Collapsed;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonPlayFilm_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //SelectedFilmPreview.Source = new Uri(FilmName);
                //SelectedFilmPreview.();

                //Output.SelectedFilmPreview.Source = new Uri(FilmName);
                Output.SelectedFilmPreview.Play();

                Output.SelectedFilmPreview.Visibility = Visibility.Visible;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonPreviousFilm_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedFilmPreview.Pause();

            Output.SelectedFilmPreview.Pause();
        }

        #endregion

        private void ButtonRestartFilm_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( FilmName != null )
                {
                    Output.SelectedFilmPreview.Source = new Uri(FilmName);
                    SelectedFilmPreview.Source        = new Uri(FilmName);
                }

                Output.SelectedFilmPreview.Play();
                SelectedFilmPreview.Play();


                if ( Output.SelectedFilmPreview.Visibility != Visibility.Visible )
                {
                    EnabledServiceCount++;
                    Output.StopScreenSaver();
                }

                Output.SelectedFilmPreview.Visibility = Visibility.Visible;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonResizeFilm_OnClick(object sender, RoutedEventArgs e)
        {
            if ( ResizeFilmPackIcon.Kind == PackIconKind.Fullscreen )
            {
                ResizeFilmPackIcon.Kind = PackIconKind.FullscreenExit;

                Output.SelectedFilmPreview.Stretch = Stretch.Fill;
            }
            else
            {
                ResizeFilmPackIcon.Kind = PackIconKind.Fullscreen;

                Output.SelectedFilmPreview.Stretch = Stretch.Uniform;
            }
        }

        private void SlideShowTime_OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !ControlHandlers.IsKeyNumber(e);
        }

        private void TxtSlideShowTime_OnLostFocus(object sender, RoutedEventArgs e)
        {
            /*try
            {
                Settings.Default.SlideShowTime = Convert.ToInt32(TxtSlideShowTime.Text) * 1000;
                Settings.Default.Save();

                SlideShowTimer.Interval = Settings.Default.SlideShowTime;
            }
            catch (Exception exception)
            {
                LogInformation(exception);
            }*/
            if ( string.IsNullOrWhiteSpace(TxtSlideShowTime.Text) )
            {
                TxtSlideShowTime.Text = "0";
            }
        }

        private void TxtSlideShowTime_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Settings.Default.SlideShowTime = Convert.ToInt32(TxtSlideShowTime.Text) * 1000;
                Settings.Default.Save();

                SlideShowTimer.Interval = Settings.Default.SlideShowTime;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void SlideShowImage_OnChecked(object sender, RoutedEventArgs e)
        {
            SlideShowTimer.Start();
        }

        private void SlideShowImage_OnUnchecked(object sender, RoutedEventArgs e)
        {
            SlideShowTimer.Stop();
        }

        private void LogoShowTime_OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !ControlHandlers.IsKeyNumber(e);
        }

        private void TxtLogoShowTime_OnLostFocus(object sender, RoutedEventArgs e)
        {
            /*try
            {
                Settings.Default.SlideShowTime = Convert.ToInt32(TxtSlideShowTime.Text) * 1000;
                Settings.Default.Save();

                SlideShowTimer.Interval = Settings.Default.SlideShowTime;
            }
            catch (Exception exception)
            {
                LogInformation(exception);
            }*/
            if ( string.IsNullOrWhiteSpace(TxtLogoShowTime.Text) )
            {
                TxtLogoShowTime.Text = "0";
            }
        }

        private void TxtLogoShowTime_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Settings.Default.LogoShowTime = Convert.ToInt32(TxtSlideShowTime.Text) * 1000;
                Settings.Default.Save();

                LogoShowTimer.Interval = Settings.Default.LogoShowTime;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void SlideShowLogo_OnChecked(object sender, RoutedEventArgs e)
        {
            LogoShowTimer.Start();
        }

        private void SlideShowLogo_OnUnchecked(object sender, RoutedEventArgs e)
        {
            LogoShowTimer.Stop();
        }

        private void MaximizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if ( MaximizeIcon.Kind == PackIconKind.WindowMaximize )
            {
                WindowState       = WindowState.Maximized;
                MaximizeIcon.Kind = PackIconKind.WindowRestore;
            }
            else
            {
                WindowState       = WindowState.Normal;
                MaximizeIcon.Kind = PackIconKind.WindowMaximize;
            }
        }

        public void ComboBoxPersonalDuaNames_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PersonalDuaListDisplayModel personalDuaList =
                    (PersonalDuaListDisplayModel) ComboBoxPersonalDuaNames.SelectedItem;
                DataTable farazes = PersonalDuaController.GetFarazTableByDuaId_Obsolete(personalDuaList.DuaId);

                farazes.Columns["DuaName"].ColumnName     = "T1";
                farazes.Columns["ArabicText"].ColumnName  = "H1";
                farazes.Columns["PersianText"].ColumnName = "H2";

                Output.ShowTableInPanel(Enums.PanelTypes.Mafatih.ToString(), farazes);
                Output.ReplaceTitleText(Enums.PanelTypes.Mafatih.ToString(), personalDuaList.DuaName);

                Output.ResetFaraz();

                ComboBoxPersonalDuaFarazId.Items.Clear();
                foreach ( DataRow faraz in farazes.Rows )
                {
                    ComboBoxPersonalDuaFarazId.Items.Add(faraz["FarazId"]);
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void ComboBoxPersonalDuaFarazId_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ( ComboBoxPersonalDuaFarazId.SelectedIndex == -1 )
                {
                    return;
                }

                Output.ResetFaraz(ComboBoxPersonalDuaFarazId.SelectedIndex);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void ButtonCustomPersonalDuaSettings_OnClick(object sender, RoutedEventArgs e)
        {
            ClearPage();
            PersonalDua.Visibility = Visibility.Visible;
        }

        public void ButtonPersonalDuaNextFaraz_OnClick(object sender, RoutedEventArgs e)
        {
            Output.SetNextFaraz();
        }

        public void ButtonPersonalDuaPreviousFaraz_OnClick(object sender, RoutedEventArgs e)
        {
            Output.SetPreviousFaraz();
        }

        public void ButtonPersonalDuaRestartFaraz_OnClick(object sender, RoutedEventArgs e)
        {
            Output.ResetFaraz();
        }

        public async void PersonalDuaPlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( ComboBoxPersonalDuaNames.SelectedIndex == -1 )
                {
                    return;
                }

                string panelName = Enums.PanelTypes.Mafatih.ToString();

                if ( PersonalDuaPlayPackIcon.Kind == PlayIcon )
                {
                    PersonalDuaPlayPackIcon.Kind = PauseIcon;

                    PersonalDuaListDisplayModel selectedPersonalDuaList =
                        (PersonalDuaListDisplayModel) ComboBoxPersonalDuaNames.SelectedItem;
                    int       duaId   = selectedPersonalDuaList.DuaId;
                    DataTable farazes = PersonalDuaController.GetFarazTableByDuaId_Obsolete(duaId);

                    if ( UserConfig.MafatihAnimationMode == Enums.AnimationMode.Static.ToString() )
                    {
                        Output.MafatihMarquee.HorizontalAlignment = HorizontalAlignment.Center;
                        Output.MafatihMarquee.TextAlignment       = TextAlignment.Center;
                        Output.MafatihMarquee.TextWrapping        = TextWrapping.Wrap;
                        Output.MafatihMarquee.FlowDirection       = FlowDirection.RightToLeft;

                        Output.MafatihTranslationMarquee.TextAlignment = TextAlignment.Center;
                    }
                    else
                    {
                        Output.MafatihMarquee.HorizontalAlignment = HorizontalAlignment.Left;
                        Output.MafatihMarquee.TextWrapping        = TextWrapping.NoWrap;
                        Output.MafatihMarquee.FlowDirection       = FlowDirection.LeftToRight;
                        await Output.StartAnimation(panelName);
                    }

                    farazes.Columns["DuaName"].ColumnName     = "T1";
                    farazes.Columns["ArabicText"].ColumnName  = "H1";
                    farazes.Columns["PersianText"].ColumnName = "H2";

                    Output.ShowTableInPanel(panelName, farazes);
                    Output.ReplaceTitleText(panelName,
                                            $"{selectedPersonalDuaList.DuaName} ({OutputWindowView.MafatihOutputTable.Rows[OutputWindowView.MafatihRowCounter]["FarazId"]})");

                    // Output.Show();
                    Output.MafatihContainer.Visibility            = Visibility.Visible;
                    Output.MafatihTranslationContainer.Visibility = Visibility.Visible;


                    EnabledServiceCount++;
                    Output.StopScreenSaver();
                }
                else
                {
                    PersonalDuaPlayPackIcon.Kind = PlayIcon;

                    Output.MafatihContainer.Visibility            = Visibility.Collapsed;
                    Output.MafatihTranslationContainer.Visibility = Visibility.Collapsed;

                    Output.StopAnimation(panelName);

                    EnabledServiceCount--;
                    Output.StartScreenSaver();
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }
    }
}