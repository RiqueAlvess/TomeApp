using TomeApp.Mobile.ViewModels;

namespace TomeApp.Mobile.Views;

public partial class TimerPage : ContentPage
{
    private readonly TimerViewModel _vm;

    public TimerPage(TimerViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadReadingBooksAsync();
    }
}
