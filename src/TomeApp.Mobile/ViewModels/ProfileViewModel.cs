using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TomeApp.Mobile.Services;

namespace TomeApp.Mobile.ViewModels;

public partial class ProfileViewModel(ApiService api, AuthService auth) : ObservableObject
{
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string? _avatarUrl;
    [ObservableProperty] private string? _bio;
    [ObservableProperty] private int _totalBooksRead;
    [ObservableProperty] private int _totalPagesRead;
    [ObservableProperty] private double _averageRating;

    [RelayCommand]
    public async Task LoadProfileAsync()
    {
        Name = auth.UserName ?? string.Empty;
        Email = auth.UserEmail ?? string.Empty;

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return;

        IsLoading = true;
        try
        {
            var profile = await api.GetProfileAsync();
            if (profile is null) return;
            Name = profile.Name;
            Email = profile.Email;
            AvatarUrl = profile.AvatarUrl;
            Bio = profile.Bio;
            TotalBooksRead = profile.Stats.TotalBooksRead;
            TotalPagesRead = profile.Stats.TotalPagesRead;
            AverageRating = profile.Stats.AverageRating;
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public void Logout()
    {
        auth.Logout();
        Application.Current!.MainPage = new Views.LoginPage();
    }
}
