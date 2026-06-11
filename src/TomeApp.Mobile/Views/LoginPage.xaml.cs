using TomeApp.Mobile.Services;

namespace TomeApp.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _api;
    private readonly AuthService _auth;

    public LoginPage()
    {
        InitializeComponent();
        _api = Handler?.MauiContext?.Services.GetRequiredService<ApiService>()
               ?? new ApiService(new AuthService());
        _auth = Handler?.MauiContext?.Services.GetRequiredService<AuthService>()
                ?? new AuthService();
    }

    private void ShowLogin(object sender, TappedEventArgs e)
    {
        LoginForm.IsVisible = true;
        RegisterForm.IsVisible = false;
        LoginTab.BackgroundColor = Color.FromArgb("#b32349");
        RegisterTab.BackgroundColor = Colors.Transparent;
    }

    private void ShowRegister(object sender, TappedEventArgs e)
    {
        LoginForm.IsVisible = false;
        RegisterForm.IsVisible = true;
        LoginTab.BackgroundColor = Colors.Transparent;
        RegisterTab.BackgroundColor = Color.FromArgb("#b32349");
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        LoginError.IsVisible = false;
        if (string.IsNullOrWhiteSpace(LoginEmail.Text) || string.IsNullOrWhiteSpace(LoginPassword.Text))
        {
            LoginError.Text = "Preencha e-mail e senha.";
            LoginError.IsVisible = true;
            return;
        }
        SetLoading(true);
        var result = await _api.LoginAsync(LoginEmail.Text.Trim(), LoginPassword.Text);
        SetLoading(false);
        if (result is null)
        {
            LoginError.Text = "E-mail ou senha inválidos.";
            LoginError.IsVisible = true;
            return;
        }
        _auth.SaveAuth(result.Token, result.UserId, result.Name, result.Email);
        Application.Current!.MainPage = new AppShell();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        RegError.IsVisible = false;
        if (string.IsNullOrWhiteSpace(RegName.Text) || string.IsNullOrWhiteSpace(RegEmail.Text) || string.IsNullOrWhiteSpace(RegPassword.Text))
        {
            RegError.Text = "Preencha todos os campos.";
            RegError.IsVisible = true;
            return;
        }
        if (RegPassword.Text.Length < 8)
        {
            RegError.Text = "A senha deve ter no mínimo 8 caracteres.";
            RegError.IsVisible = true;
            return;
        }
        SetLoading(true);
        var result = await _api.RegisterAsync(RegEmail.Text.Trim(), RegName.Text.Trim(), RegPassword.Text);
        SetLoading(false);
        if (result is null)
        {
            RegError.Text = "Erro ao criar conta. E-mail já cadastrado?";
            RegError.IsVisible = true;
            return;
        }
        _auth.SaveAuth(result.Token, result.UserId, result.Name, result.Email);
        Application.Current!.MainPage = new AppShell();
    }

    private void SetLoading(bool loading)
    {
        LoadingIndicator.IsRunning = loading;
        LoadingIndicator.IsVisible = loading;
        LoginForm.IsEnabled = !loading;
        RegisterForm.IsEnabled = !loading;
    }
}
