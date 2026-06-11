using TomeApp.Mobile.Models;
using TomeApp.Mobile.Services;

namespace TomeApp.Mobile.Views;

[QueryProperty(nameof(BookId), "id")]
public partial class BookDetailPage : ContentPage
{
    private readonly LocalDatabaseService _localDb;
    private LocalBook? _book;

    public string? BookId { get; set; }

    public BookDetailPage(LocalDatabaseService localDb)
    {
        InitializeComponent();
        _localDb = localDb;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BookId is null) return;

        _book = await _localDb.GetBookAsync(Guid.Parse(BookId));
        if (_book is null) { await Shell.Current.GoToAsync(".."); return; }

        BindingContext = _book;
        var sessions = await _localDb.GetSessionsForBookAsync(_book.Id);
        SessionsList.ItemsSource = sessions;
    }

    private async void OnReadNowClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//timer");

    private async void OnEditClicked(object sender, EventArgs e)
        => await DisplayAlert("Em breve", "Edição de livro disponível em breve.", "OK");

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_book is null) return;
        var confirm = await DisplayAlert("Remover livro", $"Deseja remover \"{_book.Title}\" da sua biblioteca?", "Remover", "Cancelar");
        if (!confirm) return;
        await _localDb.DeleteBookAsync(_book.Id);
        await Shell.Current.GoToAsync("..");
    }
}
