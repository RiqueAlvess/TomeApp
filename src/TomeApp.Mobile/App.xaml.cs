using TomeApp.Mobile.Services;

namespace TomeApp.Mobile;

public partial class App : Application
{
    public App(AuthService auth)
    {
        InitializeComponent();
        MainPage = auth.IsAuthenticated ? new AppShell() : new Views.LoginPage();
    }
}
