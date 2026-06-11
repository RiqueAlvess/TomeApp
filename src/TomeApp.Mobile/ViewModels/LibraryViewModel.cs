using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TomeApp.Mobile.Models;
using TomeApp.Mobile.Services;

namespace TomeApp.Mobile.ViewModels;

public partial class LibraryViewModel(LocalDatabaseService localDb, SyncService sync) : ObservableObject
{
    [ObservableProperty] private ObservableCollection<LocalBook> _books = [];
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _selectedFilter = "Todos";

    public string[] Filters { get; } = ["Todos", "Lendo", "Lidos", "Quero ler", "Abandonados", "Favoritos"];

    [RelayCommand]
    public async Task LoadBooksAsync()
    {
        IsLoading = true;
        try
        {
            var status = SelectedFilter switch
            {
                "Lendo" => (BookStatus?)BookStatus.Reading,
                "Lidos" => BookStatus.Read,
                "Quero ler" => BookStatus.WantToRead,
                "Abandonados" => BookStatus.Abandoned,
                "Favoritos" => BookStatus.Favorite,
                _ => null
            };
            var list = await localDb.GetBooksAsync(status);
            Books = new ObservableCollection<LocalBook>(list);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task ApplyFilterAsync(string filter)
    {
        SelectedFilter = filter;
        await LoadBooksAsync();
    }

    [RelayCommand]
    public async Task TrySyncAsync()
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            await sync.SyncPendingSessionsAsync();
    }

    [RelayCommand]
    public async Task DeleteBookAsync(LocalBook book)
    {
        await localDb.DeleteBookAsync(book.Id);
        Books.Remove(book);
    }
}
