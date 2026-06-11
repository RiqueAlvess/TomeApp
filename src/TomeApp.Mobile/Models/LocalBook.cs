using SQLite;

namespace TomeApp.Mobile.Models;

public enum BookStatus { WantToRead, Reading, Read, Abandoned, Favorite }

[Table("Books")]
public class LocalBook
{
    [PrimaryKey] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ServerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public string? CoverUrl { get; set; }
    public string? Genre { get; set; }
    public BookStatus Status { get; set; } = BookStatus.WantToRead;
    public float? Rating { get; set; }
    public bool IsSynced { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Ignore] public int ProgressPercent =>
        TotalPages > 0 ? (int)Math.Round((double)CurrentPage / TotalPages * 100) : 0;
}
