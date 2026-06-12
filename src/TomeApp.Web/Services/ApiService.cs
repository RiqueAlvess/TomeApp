using System.Net.Http.Json;
using TomeApp.Web.Models;

namespace TomeApp.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    // Books
    public async Task<List<BookDto>> GetBooksAsync()
        => await _http.GetFromJsonAsync<List<BookDto>>("api/v1/books") ?? [];

    public async Task<BookDto?> GetBookAsync(Guid id)
        => await _http.GetFromJsonAsync<BookDto>($"api/v1/books/{id}");

    public async Task<BookDto?> AddBookAsync(AddBookRequest req)
    {
        var resp = await _http.PostAsJsonAsync("api/v1/books", req);
        return resp.IsSuccessStatusCode ? await resp.Content.ReadFromJsonAsync<BookDto>() : null;
    }

    public async Task<bool> UpdateProgressAsync(Guid bookId, int currentPage)
    {
        var resp = await _http.PatchAsJsonAsync($"api/v1/books/{bookId}/progress", new UpdateProgressRequest(currentPage));
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteBookAsync(Guid id)
    {
        var resp = await _http.DeleteAsync($"api/v1/books/{id}");
        return resp.IsSuccessStatusCode;
    }

    // Notes
    public async Task<List<NoteDto>> GetNotesAsync(Guid bookId)
        => await _http.GetFromJsonAsync<List<NoteDto>>($"api/v1/books/{bookId}/notes") ?? [];

    public async Task<bool> AddNoteAsync(Guid bookId, AddNoteRequest req)
    {
        var resp = await _http.PostAsJsonAsync($"api/v1/books/{bookId}/notes", req);
        return resp.IsSuccessStatusCode;
    }

    // Ranking
    public async Task<List<RankingEntry>> GetRankingAsync(int year, int month)
        => await _http.GetFromJsonAsync<List<RankingEntry>>($"api/v1/ranking?year={year}&month={month}") ?? [];

    // Profile
    public async Task<UserProfile?> GetProfileAsync()
        => await _http.GetFromJsonAsync<UserProfile>("api/v1/profile");

    // Sync
    public async Task<bool> SyncSessionsAsync(List<SyncSessionRequest> sessions)
    {
        var resp = await _http.PostAsJsonAsync("api/v1/sync", sessions);
        return resp.IsSuccessStatusCode;
    }
}
