using TomeApp.Mobile.ViewModels;

namespace TomeApp.Mobile.Views;

public partial class LibraryPage : ContentPage
{
    private readonly LibraryViewModel _vm;

    public LibraryPage(LibraryViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadBooksAsync();
        await _vm.TrySyncAsync();
    }

    private async void OnAddBookClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("addbook");

    private async void OnBookTapped(object sender, EventArgs e)
    {
        if (sender is BindableObject b && b.BindingContext is Models.LocalBook book)
            await Shell.Current.GoToAsync($"bookdetail?id={book.Id}");
    }
}
