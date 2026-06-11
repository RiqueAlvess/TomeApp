using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TomeApp.Admin.Models;

[Table("Users")]
public class UserView
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = "BR";
    public DateTime CreatedAt { get; set; }
}

[Table("ChampionshipEntries")]
public class ChampionshipView
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalMinutesRead { get; set; }
    public int Rank { get; set; }
    public bool PrizeAwarded { get; set; }
    public DateTime? PrizeAwardedAt { get; set; }
    public string? PrizeAwardedBy { get; set; }
    public string Region { get; set; } = "BR";

    [NotMapped]
    public string? UserName { get; set; }
    [NotMapped]
    public string? UserEmail { get; set; }
}

[Table("ReadingSessions")]
public class ReadingSessionView
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MinutesRead { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public DateTime DeviceRecordedAt { get; set; }
    public DateTime ServerReceivedAt { get; set; }
}
