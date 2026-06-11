using TomeApp.API.Models.Entities;

namespace TomeApp.API.Models.DTOs;

public record CreateBookRequest(
    string Title,
    string Author,
    int TotalPages,
    string? CoverUrl,
    string? Genre,
    BookStatus Status = BookStatus.WantToRead
);

public record UpdateBookRequest(
    string? Title,
    string? Author,
    int? TotalPages,
    int? CurrentPage,
    string? CoverUrl,
    string? Genre,
    BookStatus? Status,
    float? Rating
);

public record BookResponse(
    Guid Id,
    string Title,
    string Author,
    int TotalPages,
    int CurrentPage,
    string? CoverUrl,
    string? Genre,
    BookStatus Status,
    float? Rating,
    int ProgressPercent,
    DateTime CreatedAt
);
