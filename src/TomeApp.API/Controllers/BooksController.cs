using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomeApp.API.Data;
using TomeApp.API.Models.DTOs;
using TomeApp.API.Models.Entities;

namespace TomeApp.API.Controllers;

[ApiController]
[Route("api/v1/books")]
[Authorize]
public class BooksController(AppDbContext db) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IList<BookResponse>>> GetBooks([FromQuery] BookStatus? status)
    {
        var query = db.Books.Where(b => b.UserId == UserId);
        if (status.HasValue) query = query.Where(b => b.Status == status.Value);

        var books = await query.OrderByDescending(b => b.UpdatedAt).ToListAsync();
        return Ok(books.Select(MapToResponse));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponse>> GetBook(Guid id)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id && b.UserId == UserId);
        return book is null ? NotFound() : Ok(MapToResponse(book));
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> CreateBook(CreateBookRequest req)
    {
        var book = new Book
        {
            UserId = UserId,
            Title = req.Title,
            Author = req.Author,
            TotalPages = req.TotalPages,
            CoverUrl = req.CoverUrl,
            Genre = req.Genre,
            Status = req.Status
        };

        db.Books.Add(book);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, MapToResponse(book));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookResponse>> UpdateBook(Guid id, UpdateBookRequest req)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id && b.UserId == UserId);
        if (book is null) return NotFound();

        if (req.Title is not null) book.Title = req.Title;
        if (req.Author is not null) book.Author = req.Author;
        if (req.TotalPages.HasValue) book.TotalPages = req.TotalPages.Value;
        if (req.CurrentPage.HasValue) book.CurrentPage = req.CurrentPage.Value;
        if (req.CoverUrl is not null) book.CoverUrl = req.CoverUrl;
        if (req.Genre is not null) book.Genre = req.Genre;
        if (req.Status.HasValue) book.Status = req.Status.Value;
        if (req.Rating.HasValue) book.Rating = req.Rating.Value;
        book.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Ok(MapToResponse(book));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == id && b.UserId == UserId);
        if (book is null) return NotFound();

        db.Books.Remove(book);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static BookResponse MapToResponse(Book b) => new(
        b.Id, b.Title, b.Author, b.TotalPages, b.CurrentPage,
        b.CoverUrl, b.Genre, b.Status, b.Rating,
        b.TotalPages > 0 ? (int)Math.Round((double)b.CurrentPage / b.TotalPages * 100) : 0,
        b.CreatedAt
    );
}
