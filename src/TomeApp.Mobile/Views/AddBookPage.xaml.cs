using TomeApp.Mobile.Models;
using TomeApp.Mobile.Services;

namespace TomeApp.Mobile.Views;

public partial class AddBookPage : ContentPage
{
    private readonly LocalDatabaseService _localDb;
    private readonly ApiService _api;

    public AddBookPage(LocalDatabaseService localDb, ApiService api)
    {
        InitializeComponent();
        _localDb = localDb;
        _api = api;
        StatusPicker.SelectedIndex = 0;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(TitleEntry.Text) ||
            string.IsNullOrWhiteSpace(AuthorEntry.Text) ||
            !int.TryParse(PagesEntry.Text, out var pages) || pages < 1)
        {
            ErrorLabel.Text = "Preencha título, autor e número de páginas válido.";
            ErrorLabel.IsVisible = true;
            return;
        }

        var status = StatusPicker.SelectedIndex switch
        {
            1 => BookStatus.Reading,
            2 => BookStatus.Read,
            _ => BookStatus.WantToRead
        };

        var book = new LocalBook
        {
            Title = TitleEntry.Text.Trim(),
            Author = AuthorEntry.Text.Trim(),
            TotalPages = pages,
            Genre = GenreEntry.Text?.Trim(),
            Status = status,
            IsSynced = false
        };

        await _localDb.SaveBookAsync(book);

        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            var serverBook = await _api.CreateBookAsync(book.Title, book.Author, book.TotalPages, book.Genre);
            if (serverBook is not null)
            {
                book.ServerId = serverBook.Id;
                book.IsSynced = true;
                await _localDb.SaveBookAsync(book);
            }
        }

        await Shell.Current.GoToAsync("..");
    }
}
