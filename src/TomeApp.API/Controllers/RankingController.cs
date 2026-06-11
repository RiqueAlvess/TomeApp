using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomeApp.API.Data;

namespace TomeApp.API.Controllers;

[ApiController]
[Route("api/v1/ranking")]
[Authorize]
public class RankingController(AppDbContext db) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetRanking([FromQuery] int? year, [FromQuery] int? month)
    {
        var now = DateTime.UtcNow;
        var y = year ?? now.Year;
        var m = month ?? now.Month;

        var entries = await db.ChampionshipEntries
            .Include(e => e.User)
            .Where(e => e.Year == y && e.Month == m && e.Region == "BR")
            .OrderByDescending(e => e.TotalMinutesRead)
            .Take(100)
            .Select((e, i) => new
            {
                Rank = i + 1,
                e.UserId,
                e.User!.Name,
                e.User.AvatarUrl,
                e.TotalMinutesRead,
                e.PrizeAwarded
            })
            .ToListAsync();

        var myEntry = entries.FirstOrDefault(e => e.UserId == UserId);
        var daysLeft = DateTime.DaysInMonth(y, m) - now.Day;

        return Ok(new
        {
            Year = y,
            Month = m,
            DaysLeft = m == now.Month && y == now.Year ? daysLeft : 0,
            MyRank = myEntry?.Rank,
            MyMinutes = myEntry?.TotalMinutesRead ?? 0,
            Entries = entries
        });
    }
}
