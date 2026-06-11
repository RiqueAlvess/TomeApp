using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomeApp.API.Data;
using TomeApp.API.Models.Entities;

namespace TomeApp.API.Controllers;

[ApiController]
[Route("api/v1/books/{bookId}/notes")]
[Authorize]
public class NotesController(AppDbContext db) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetNotes(Guid bookId)
    {
        var notes = await db.Notes
            .Where(n => n.BookId == bookId && n.UserId == UserId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return Ok(notes);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote(Guid bookId, [FromBody] CreateNoteRequest req)
    {
        var bookExists = await db.Books.AnyAsync(b => b.Id == bookId && b.UserId == UserId);
        if (!bookExists) return NotFound();

        var note = new Note
        {
            UserId = UserId,
            BookId = bookId,
            Type = req.Type,
            Content = req.Content,
            PageNumber = req.PageNumber
        };

        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return Ok(note);
    }

    [HttpDelete("{noteId}")]
    public async Task<IActionResult> DeleteNote(Guid bookId, Guid noteId)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == noteId && n.BookId == bookId && n.UserId == UserId);
        if (note is null) return NotFound();

        db.Notes.Remove(note);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

public record CreateNoteRequest(NoteType Type, string Content, int? PageNumber);
