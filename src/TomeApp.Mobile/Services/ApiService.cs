using System.Net.Http.Json;
using System.Text.Json;

namespace TomeApp.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly AuthService _auth;

    public ApiService(AuthService auth)
    {
        _auth = auth;
        _http = new HttpClient
        {
            BaseAddress = new Uri(AppSettings.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    private void AttachToken()
    {
        _http.DefaultRequestHeaders.Authorization = _auth.Token is not null
            ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.Token)
            : null;
    }

    public async Task<AuthResult?> RegisterAsync(string email, string name, string password)
    {
        var resp = await _http.PostAsJsonAsync("api/v1/auth/register", new { email, name, password, region = "BR" });
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<AuthResult>();
    }

    public async Task<AuthResult?> LoginAsync(string email, string password)
    {
        var resp = await _http.PostAsJsonAsync("api/v1/auth/login", new { email, password });
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<AuthResult>();
    }

    public async Task<List<ApiBook>> GetBooksAsync()
    {
        AttachToken();
        try
        {
            var result = await _http.GetFromJsonAsync<List<ApiBook>>("api/v1/books");
            return result ?? [];
        }
        catch { return []; }
    }

    public async Task<ApiBook?> CreateBookAsync(string title, string author, int totalPages, string? genre)
    {
        AttachToken();
        try
        {
            var resp = await _http.PostAsJsonAsync("api/v1/books", new { title, author, totalPages, genre });
            return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<ApiBook>() : null;
        }
        catch { return null; }
    }

    public async Task<bool> SyncSessionsAsync(IList<SyncItem> sessions)
    {
        AttachToken();
        try
        {
            var resp = await _http.PostAsJsonAsync("api/v1/sync", new { sessions });
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<RankingResult?> GetRankingAsync()
    {
        AttachToken();
        try { return await _http.GetFromJsonAsync<RankingResult>("api/v1/ranking"); }
        catch { return null; }
    }

    public async Task<ProfileResult?> GetProfileAsync()
    {
        AttachToken();
        try { return await _http.GetFromJsonAsync<ProfileResult>("api/v1/profile"); }
        catch { return null; }
    }
}

public record AuthResult(string Token, string UserId, string Name, string Email);
public record ApiBook(Guid Id, string Title, string Author, int TotalPages, int CurrentPage, string? CoverUrl, string? Genre, string Status, float? Rating, int ProgressPercent, DateTime CreatedAt);
public record SyncItem(Guid LocalId, Guid BookId, DateTime StartTime, DateTime EndTime, int MinutesRead, int StartPage, int EndPage, DateTime DeviceRecordedAt);
public record RankingResult(int Year, int Month, int DaysLeft, int? MyRank, int MyMinutes, List<RankingEntry> Entries);
public record RankingEntry(int Rank, Guid UserId, string Name, string? AvatarUrl, int TotalMinutesRead, bool PrizeAwarded);
public record ProfileResult(Guid Id, string Name, string Email, string? AvatarUrl, string? Bio, ProfileStats Stats);
public record ProfileStats(int TotalBooksRead, int TotalPagesRead, double AverageRating);
