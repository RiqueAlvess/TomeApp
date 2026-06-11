using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TomeApp.API.Data;
using TomeApp.API.Models.DTOs;
using TomeApp.API.Models.Entities;

namespace TomeApp.API.Services;

public class SyncProcessorService(
    IServiceScopeFactory scopeFactory,
    RabbitMQService rabbitMQ,
    ILogger<SyncProcessorService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Sync processor started");

        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        try
        {
            await rabbitMQ.StartConsumingAsync(ProcessMessageAsync, stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Sync processor failed to start consuming");
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessMessageAsync(string json)
    {
        var batch = JsonSerializer.Deserialize<SyncBatchMessage>(json);
        if (batch is null) return;

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var item in batch.Sessions)
        {
            var session = new ReadingSession
            {
                Id = Guid.NewGuid(),
                UserId = batch.UserId,
                BookId = item.BookId,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                MinutesRead = item.MinutesRead,
                StartPage = item.StartPage,
                EndPage = item.EndPage,
                DeviceRecordedAt = item.DeviceRecordedAt,
                ServerReceivedAt = DateTime.UtcNow
            };

            db.ReadingSessions.Add(session);

            var book = await db.Books.FindAsync(item.BookId);
            if (book != null && item.EndPage > book.CurrentPage)
            {
                book.CurrentPage = item.EndPage;
                book.UpdatedAt = DateTime.UtcNow;
            }
        }

        await db.SaveChangesAsync();
        await UpdateChampionshipAsync(db, batch.UserId);
        logger.LogInformation("Processed {Count} sessions for user {UserId}", batch.Sessions.Count, batch.UserId);
    }

    private static async Task UpdateChampionshipAsync(AppDbContext db, Guid userId)
    {
        var now = DateTime.UtcNow;
        var monthMinutes = await db.ReadingSessions
            .Where(s => s.UserId == userId && s.DeviceRecordedAt.Year == now.Year && s.DeviceRecordedAt.Month == now.Month)
            .SumAsync(s => s.MinutesRead);

        var user = await db.Users.FindAsync(userId);
        var entry = await db.ChampionshipEntries
            .FirstOrDefaultAsync(e => e.UserId == userId && e.Year == now.Year && e.Month == now.Month);

        if (entry is null)
        {
            db.ChampionshipEntries.Add(new ChampionshipEntry
            {
                UserId = userId,
                Year = now.Year,
                Month = now.Month,
                TotalMinutesRead = monthMinutes,
                Region = user?.Region ?? "BR"
            });
        }
        else
        {
            entry.TotalMinutesRead = monthMinutes;
        }

        await db.SaveChangesAsync();
    }
}

public record SyncBatchMessage(Guid UserId, List<SyncSessionItem> Sessions);
