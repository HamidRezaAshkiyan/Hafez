using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HafezLibrary.Models;

namespace HafezWPFUI.ViewModels.Windows
{
    public partial class ListenerWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isServerRunning;

        [ObservableProperty]
        private string _endpointDisplay = string.Empty;

        [ObservableProperty]
        private int _connectedClientsCount;

        public ObservableCollection<UserModel> CommandHistory { get; } = new();

        [RelayCommand]
        private void ClearHistory()
        {
            CommandHistory.Clear();
        }
    }
}
