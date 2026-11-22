using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HafezLibrary.Models;
using HafezLibrary.Services;
using HafezLibrary.Settings;

namespace HafezWPFUI.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IContentCacheService _contentCache;

        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private bool _isNotificationPlaying;

        [ObservableProperty]
        private bool _isQuranPlaying;

        [ObservableProperty]
        private bool _isMafatihPlaying;

        [ObservableProperty]
        private bool _isPersonalDuaPlaying;

        [ObservableProperty]
        private bool _isCameraActive;

        [ObservableProperty]
        private bool _isGalleryPlaying;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private string _activeStatusText = "آماده به کار";

        public ObservableCollection<QuranModel> QuranSearchResults { get; } = new();
        public ObservableCollection<MafatihListModel> MafatihSearchResults { get; } = new();

        public MainWindowViewModel(IContentCacheService contentCache)
        {
            _contentCache = contentCache;
            AppSettings settings = AppSettingsService.Load();
            IsDarkMode = settings.DarkMode;
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            await _contentCache.WarmUpAsync();

            QuranSearchResults.Clear();
            foreach (QuranModel ayah in _contentCache.SearchQuran(SearchQuery, 30))
            {
                QuranSearchResults.Add(ayah);
            }

            MafatihSearchResults.Clear();
            foreach (MafatihListModel dua in _contentCache.SearchMafatih(SearchQuery, 30))
            {
                MafatihSearchResults.Add(dua);
            }
        }
    }
}
