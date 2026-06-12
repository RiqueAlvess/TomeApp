using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using TomeApp.Web.Models;

namespace TomeApp.Web.Services;

public class AuthService
{
    private const string TokenKey = "auth_token";
    private const string UserKey = "auth_user";

    private readonly ILocalStorageService _storage;
    private readonly HttpClient _http;

    public event Action? OnAuthChanged;
    public AuthResponse? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser is not null;

    public AuthService(ILocalStorageService storage, HttpClient http)
    {
        _storage = storage;
        _http = http;
    }

    public async Task InitializeAsync()
    {
        var token = await _storage.GetItemAsStringAsync(TokenKey);
        if (!string.IsNullOrEmpty(token))
        {
            var user = await _storage.GetItemAsync<AuthResponse>(UserKey);
            if (user is not null && !IsTokenExpired(token))
            {
                CurrentUser = user;
                SetAuthHeader(token);
            }
            else
            {
                await LogoutAsync();
            }
        }
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/v1/auth/login", new LoginRequest(email, password));
            if (!resp.IsSuccessStatusCode) return false;
            var auth = await resp.Content.ReadFromJsonAsync<AuthResponse>();
            if (auth is null) return false;
            await PersistAsync(auth);
            return true;
        }
        catch { return false; }
    }

    public async Task<bool> RegisterAsync(string name, string email, string password)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/v1/auth/register", new RegisterRequest(name, email, password));
            if (!resp.IsSuccessStatusCode) return false;
            var auth = await resp.Content.ReadFromJsonAsync<AuthResponse>();
            if (auth is null) return false;
            await PersistAsync(auth);
            return true;
        }
        catch { return false; }
    }

    public async Task LogoutAsync()
    {
        CurrentUser = null;
        _http.DefaultRequestHeaders.Authorization = null;
        await _storage.RemoveItemAsync(TokenKey);
        await _storage.RemoveItemAsync(UserKey);
        OnAuthChanged?.Invoke();
    }

    private async Task PersistAsync(AuthResponse auth)
    {
        CurrentUser = auth;
        await _storage.SetItemAsStringAsync(TokenKey, auth.Token);
        await _storage.SetItemAsync(UserKey, auth);
        SetAuthHeader(auth.Token);
        OnAuthChanged?.Invoke();
    }

    private void SetAuthHeader(string token)
        => _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    private static bool IsTokenExpired(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return true;
            var payload = parts[1];
            var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(padded));
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("exp", out var exp))
            {
                var expiry = DateTimeOffset.FromUnixTimeSeconds(exp.GetInt64());
                return expiry < DateTimeOffset.UtcNow;
            }
            return false;
        }
        catch { return true; }
    }
}
