using CommunityToolkit.Mvvm.ComponentModel;

namespace HafezWPFUI.ViewModels.Windows
{
    public partial class OutputWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _notificationTitle = string.Empty;

        [ObservableProperty]
        private string _notificationText = string.Empty;

        [ObservableProperty]
        private string _quranTitle = string.Empty;

        [ObservableProperty]
        private string _quranText = string.Empty;

        [ObservableProperty]
        private string _quranTranslationText = string.Empty;

        [ObservableProperty]
        private string _mafatihTitle = string.Empty;

        [ObservableProperty]
        private string _mafatihText = string.Empty;

        [ObservableProperty]
        private string _mafatihTranslationText = string.Empty;

        [ObservableProperty]
        private string _prayerBannerText = string.Empty;

        [ObservableProperty]
        private bool _isScreenSaverVisible;
    }
}
