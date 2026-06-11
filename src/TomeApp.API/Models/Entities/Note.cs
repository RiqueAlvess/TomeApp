namespace TomeApp.API.Models.Entities;

public enum NoteType { Quote, Reflection }

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public NoteType Type { get; set; } = NoteType.Reflection;
    public string Content { get; set; } = string.Empty;
    public int? PageNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Book? Book { get; set; }
}
