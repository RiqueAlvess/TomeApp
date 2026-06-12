namespace TomeApp.Web.Models;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Name, string Email, string Password);
public record AuthResponse(string Token, string Name, Guid UserId);

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string? CoverUrl { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public string Status { get; set; } = "want_to_read"; // want_to_read, reading, finished
    public DateTime AddedAt { get; set; }
    public double Progress => TotalPages > 0 ? (double)CurrentPage / TotalPages * 100 : 0;
}

public record AddBookRequest(string Title, string Author, int TotalPages, string? CoverUrl);
public record UpdateProgressRequest(int CurrentPage);

public class ReadingSessionDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public int MinutesRead { get; set; }
    public DateTime Date { get; set; }
    public bool Synced { get; set; }
}

public record SyncSessionRequest(Guid BookId, int StartPage, int EndPage, int MinutesRead, DateTime Date);

public class RankingEntry
{
    public int Position { get; set; }
    public string UserName { get; set; } = "";
    public int TotalMinutesRead { get; set; }
    public bool IsCurrentUser { get; set; }
}

public class UserProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public int TotalMinutesRead { get; set; }
    public int BooksFinished { get; set; }
    public int CurrentStreak { get; set; }
}

public class NoteDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public int Page { get; set; }
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public record AddNoteRequest(int Page, string Content);
