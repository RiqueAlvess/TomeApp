namespace TomeApp.Mobile.Services;

public class SyncService(LocalDatabaseService localDb, ApiService api, AuthService auth)
{
    public async Task<bool> SyncPendingSessionsAsync()
    {
        if (!auth.IsAuthenticated) return false;

        var pending = await localDb.GetUnsyncedSessionsAsync();
        if (pending.Count == 0) return true;

        var items = pending.Select(s => new SyncItem(
            s.Id, s.BookId, s.StartTime, s.EndTime,
            s.MinutesRead, s.StartPage, s.EndPage, s.DeviceRecordedAt
        )).ToList();

        var success = await api.SyncSessionsAsync(items);
        if (success)
            await localDb.MarkSessionsSyncedAsync(pending.Select(s => s.Id));

        return success;
    }

    public async Task SyncBooksFromServerAsync()
    {
        if (!auth.IsAuthenticated) return;

        var serverBooks = await api.GetBooksAsync();
        foreach (var sb in serverBooks)
        {
            var local = await localDb.GetBooksAsync();
            var existing = local.FirstOrDefault(b => b.ServerId == sb.Id);

            if (existing is null)
            {
                await localDb.SaveBookAsync(new Models.LocalBook
                {
                    ServerId = sb.Id,
                    Title = sb.Title,
                    Author = sb.Author,
                    TotalPages = sb.TotalPages,
                    CurrentPage = sb.CurrentPage,
                    CoverUrl = sb.CoverUrl,
                    Genre = sb.Genre,
                    IsSynced = true
                });
            }
        }
    }
}
