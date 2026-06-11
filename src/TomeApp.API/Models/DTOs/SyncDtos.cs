namespace TomeApp.API.Models.DTOs;

public record SyncSessionItem(
    Guid LocalId,
    Guid BookId,
    DateTime StartTime,
    DateTime EndTime,
    int MinutesRead,
    int StartPage,
    int EndPage,
    DateTime DeviceRecordedAt
);

public record SyncRequest(IList<SyncSessionItem> Sessions);

public record SyncResponse(int Processed, int Failed, IList<Guid> ProcessedLocalIds);
