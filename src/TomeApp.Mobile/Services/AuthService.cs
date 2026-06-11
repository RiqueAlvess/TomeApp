namespace TomeApp.Mobile.Services;

public class AuthService
{
    private const string TokenKey = "auth_token";
    private const string UserIdKey = "user_id";
    private const string UserNameKey = "user_name";
    private const string UserEmailKey = "user_email";

    public string? Token => Preferences.Get(TokenKey, null);
    public string? UserId => Preferences.Get(UserIdKey, null);
    public string? UserName => Preferences.Get(UserNameKey, null);
    public string? UserEmail => Preferences.Get(UserEmailKey, null);
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

    public void SaveAuth(string token, string userId, string name, string email)
    {
        Preferences.Set(TokenKey, token);
        Preferences.Set(UserIdKey, userId);
        Preferences.Set(UserNameKey, name);
        Preferences.Set(UserEmailKey, email);
    }

    public void Logout()
    {
        Preferences.Remove(TokenKey);
        Preferences.Remove(UserIdKey);
        Preferences.Remove(UserNameKey);
        Preferences.Remove(UserEmailKey);
    }
}
