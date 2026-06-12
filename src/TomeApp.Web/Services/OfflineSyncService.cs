using Blazored.LocalStorage;
using TomeApp.Web.Models;

namespace TomeApp.Web.Services;

public class OfflineSyncService
{
    private const string QueueKey = "offline_sync_queue";
    private readonly ILocalStorageService _storage;
    private readonly ApiService _api;

    public OfflineSyncService(ILocalStorageService storage, ApiService api)
    {
        _storage = storage;
        _api = api;
    }

    public async Task EnqueueSessionAsync(SyncSessionRequest session)
    {
        var queue = await GetQueueAsync();
        queue.Add(session);
        await _storage.SetItemAsync(QueueKey, queue);
    }

    public async Task<int> FlushAsync()
    {
        var queue = await GetQueueAsync();
        if (queue.Count == 0) return 0;
        var ok = await _api.SyncSessionsAsync(queue);
        if (ok)
        {
            await _storage.RemoveItemAsync(QueueKey);
            return queue.Count;
        }
        return 0;
    }

    public async Task<int> PendingCountAsync()
        => (await GetQueueAsync()).Count;

    private async Task<List<SyncSessionRequest>> GetQueueAsync()
        => await _storage.GetItemAsync<List<SyncSessionRequest>>(QueueKey) ?? [];
}
