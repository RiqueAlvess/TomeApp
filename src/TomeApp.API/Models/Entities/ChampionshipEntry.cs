namespace TomeApp.API.Models.Entities;

public class ChampionshipEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalMinutesRead { get; set; }
    public int Rank { get; set; }
    public bool PrizeAwarded { get; set; }
    public DateTime? PrizeAwardedAt { get; set; }
    public string? PrizeAwardedBy { get; set; }
    public string Region { get; set; } = "BR";

    public User? User { get; set; }
}
