using SQLite;

namespace TomeApp.Mobile.Models;

[Table("ReadingSessions")]
public class LocalReadingSession
{
    [PrimaryKey] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MinutesRead { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public DateTime DeviceRecordedAt { get; set; } = DateTime.UtcNow;
    public bool IsSynced { get; set; }
}
