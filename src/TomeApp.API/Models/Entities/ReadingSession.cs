namespace TomeApp.API.Models.Entities;

public class ReadingSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MinutesRead { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public DateTime DeviceRecordedAt { get; set; }
    public DateTime ServerReceivedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Book? Book { get; set; }
}
