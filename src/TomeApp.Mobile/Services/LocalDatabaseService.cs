using SQLite;
using TomeApp.Mobile.Models;

namespace TomeApp.Mobile.Services;

public class LocalDatabaseService
{
    private SQLiteAsyncConnection? _db;

    private async Task<SQLiteAsyncConnection> GetDb()
    {
        if (_db is not null) return _db;
        var path = Path.Combine(FileSystem.AppDataDirectory, "tomeapp.db");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<LocalBook>();
        await _db.CreateTableAsync<LocalReadingSession>();
        return _db;
    }

    public async Task<List<LocalBook>> GetBooksAsync(BookStatus? status = null)
    {
        var db = await GetDb();
        return status.HasValue
            ? await db.Table<LocalBook>().Where(b => b.Status == status).OrderByDescending(b => b.UpdatedAt).ToListAsync()
            : await db.Table<LocalBook>().OrderByDescending(b => b.UpdatedAt).ToListAsync();
    }

    public async Task<LocalBook?> GetBookAsync(Guid id)
    {
        var db = await GetDb();
        return await db.Table<LocalBook>().FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task SaveBookAsync(LocalBook book)
    {
        var db = await GetDb();
        book.UpdatedAt = DateTime.UtcNow;
        if (await db.Table<LocalBook>().FirstOrDefaultAsync(b => b.Id == book.Id) is null)
            await db.InsertAsync(book);
        else
            await db.UpdateAsync(book);
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var db = await GetDb();
        await db.DeleteAsync<LocalBook>(id);
    }

    public async Task SaveSessionAsync(LocalReadingSession session)
    {
        var db = await GetDb();
        await db.InsertAsync(session);
    }

    public async Task<List<LocalReadingSession>> GetUnsyncedSessionsAsync()
    {
        var db = await GetDb();
        return await db.Table<LocalReadingSession>().Where(s => !s.IsSynced).ToListAsync();
    }

    public async Task MarkSessionsSyncedAsync(IEnumerable<Guid> ids)
    {
        var db = await GetDb();
        foreach (var id in ids)
        {
            var session = await db.Table<LocalReadingSession>().FirstOrDefaultAsync(s => s.Id == id);
            if (session is null) continue;
            session.IsSynced = true;
            await db.UpdateAsync(session);
        }
    }

    public async Task<List<LocalReadingSession>> GetSessionsForBookAsync(Guid bookId)
    {
        var db = await GetDb();
        return await db.Table<LocalReadingSession>().Where(s => s.BookId == bookId).OrderByDescending(s => s.DeviceRecordedAt).ToListAsync();
    }

    public async Task<(int totalMinutes, int sessionsCount)> GetWeekStatsAsync()
    {
        var db = await GetDb();
        var since = DateTime.UtcNow.AddDays(-7);
        var sessions = await db.Table<LocalReadingSession>().Where(s => s.DeviceRecordedAt >= since).ToListAsync();
        return (sessions.Sum(s => s.MinutesRead), sessions.Count);
    }
}
