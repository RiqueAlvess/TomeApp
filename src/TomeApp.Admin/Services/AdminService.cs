using Microsoft.EntityFrameworkCore;
using TomeApp.Admin.Data;
using TomeApp.Admin.Models;

namespace TomeApp.Admin.Services;

public class AdminService(AdminDbContext db)
{
    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var totalUsers = await db.Users.CountAsync();
        var today = DateTime.UtcNow.Date;
        var activeToday = await db.ReadingSessions.Where(s => s.DeviceRecordedAt.Date == today).Select(s => s.UserId).Distinct().CountAsync();
        var totalSessions = await db.ReadingSessions.CountAsync();
        var totalMinutes = await db.ReadingSessions.SumAsync(s => s.MinutesRead);

        return new DashboardStats(totalUsers, activeToday, totalSessions, totalMinutes);
    }

    public async Task<List<ChampionshipView>> GetMonthlyRankingAsync(int year, int month)
    {
        var entries = await db.Championships
            .Where(e => e.Year == year && e.Month == month && e.Region == "BR")
            .OrderByDescending(e => e.TotalMinutesRead)
            .ToListAsync();

        var userIds = entries.Select(e => e.UserId).ToList();
        var users = await db.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id);

        foreach (var entry in entries)
        {
            if (users.TryGetValue(entry.UserId, out var user))
            {
                entry.UserName = user.Name;
                entry.UserEmail = user.Email;
            }
        }

        return entries;
    }

    public async Task MarkPrizeAwardedAsync(Guid entryId, string adminEmail)
    {
        var entry = await db.Championships.FindAsync(entryId);
        if (entry is null) return;

        entry.PrizeAwarded = true;
        entry.PrizeAwardedAt = DateTime.UtcNow;
        entry.PrizeAwardedBy = adminEmail;
        await db.SaveChangesAsync();
    }

    public async Task<List<UserSessionAudit>> GetUserAuditAsync(Guid userId, int year, int month)
    {
        var sessions = await db.ReadingSessions
            .Where(s => s.UserId == userId && s.DeviceRecordedAt.Year == year && s.DeviceRecordedAt.Month == month)
            .OrderBy(s => s.DeviceRecordedAt)
            .ToListAsync();

        return sessions.Select(s => new UserSessionAudit(
            s.DeviceRecordedAt, s.StartPage, s.EndPage, s.MinutesRead,
            s.MinutesRead > 480 ? "SUSPEITO: mais de 8h numa sessão" : null
        )).ToList();
    }

    public async Task<List<UserView>> GetUsersAsync(string? search = null)
    {
        var query = db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
        return await query.OrderByDescending(u => u.CreatedAt).Take(100).ToListAsync();
    }
}

public record DashboardStats(int TotalUsers, int ActiveToday, int TotalSessions, int TotalMinutesRead);
public record UserSessionAudit(DateTime Date, int StartPage, int EndPage, int MinutesRead, string? SuspiciousFlag);
