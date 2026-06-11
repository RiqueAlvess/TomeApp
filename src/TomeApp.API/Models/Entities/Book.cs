namespace TomeApp.API.Models.Entities;

public enum BookStatus
{
    WantToRead,
    Reading,
    Read,
    Abandoned,
    Favorite
}

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public string? CoverUrl { get; set; }
    public string? Genre { get; set; }
    public BookStatus Status { get; set; } = BookStatus.WantToRead;
    public float? Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public ICollection<ReadingSession> ReadingSessions { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
}
