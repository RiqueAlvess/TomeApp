using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomeApp.API.Data;
using TomeApp.API.Models.DTOs;

namespace TomeApp.API.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
public class ProfileController(AppDbContext db) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var user = await db.Users.FindAsync(UserId);
        if (user is null) return NotFound();

        var totalBooks = await db.Books.CountAsync(b => b.UserId == UserId && b.Status == Models.Entities.BookStatus.Read);
        var totalPages = await db.ReadingSessions.Where(s => s.UserId == UserId).SumAsync(s => s.EndPage - s.StartPage);
        var avgRating = await db.Books.Where(b => b.UserId == UserId && b.Rating.HasValue).AverageAsync(b => (double?)b.Rating) ?? 0;

        var weekSessions = await db.ReadingSessions
            .Where(s => s.UserId == UserId && s.DeviceRecordedAt >= DateTime.UtcNow.AddDays(-7))
            .GroupBy(s => s.DeviceRecordedAt.Date)
            .Select(g => new { Date = g.Key, Minutes = g.Sum(s => s.MinutesRead) })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.AvatarUrl,
            user.Bio,
            user.Region,
            Stats = new { TotalBooksRead = totalBooks, TotalPagesRead = totalPages, AverageRating = Math.Round(avgRating, 1) },
            WeekActivity = weekSessions
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest req)
    {
        var user = await db.Users.FindAsync(UserId);
        if (user is null) return NotFound();

        if (req.Name is not null) user.Name = req.Name;
        if (req.Bio is not null) user.Bio = req.Bio;
        if (req.AvatarUrl is not null) user.AvatarUrl = req.AvatarUrl;

        await db.SaveChangesAsync();
        return Ok(new { user.Name, user.Bio, user.AvatarUrl });
    }
}
