using TomeApp.Mobile.ViewModels;

namespace TomeApp.Mobile.Views;

public partial class RankingPage : ContentPage
{
    private readonly RankingViewModel _vm;

    public RankingPage(RankingViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadRankingAsync();
    }
}
