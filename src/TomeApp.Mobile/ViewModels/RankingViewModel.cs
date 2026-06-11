using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TomeApp.Mobile.Services;

namespace TomeApp.Mobile.ViewModels;

public partial class RankingViewModel(ApiService api) : ObservableObject
{
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private int _myRank;
    [ObservableProperty] private int _myMinutes;
    [ObservableProperty] private int _daysLeft;
    [ObservableProperty] private ObservableCollection<RankingEntry> _entries = [];
    [ObservableProperty] private string _statusMessage = string.Empty;

    [RelayCommand]
    public async Task LoadRankingAsync()
    {
        IsLoading = true;
        StatusMessage = string.Empty;
        try
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                StatusMessage = "Sem conexão. Conecte-se para ver o ranking.";
                return;
            }
            var result = await api.GetRankingAsync();
            if (result is null) { StatusMessage = "Erro ao carregar ranking."; return; }

            MyRank = result.MyRank ?? 0;
            MyMinutes = result.MyMinutes;
            DaysLeft = result.DaysLeft;
            Entries = new ObservableCollection<RankingEntry>(result.Entries);
        }
        finally { IsLoading = false; }
    }
}
