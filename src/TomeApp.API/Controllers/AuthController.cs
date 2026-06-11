using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomeApp.API.Data;
using TomeApp.API.Models.DTOs;
using TomeApp.API.Models.Entities;
using TomeApp.API.Services;

namespace TomeApp.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(AppDbContext db, TokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        if (await db.Users.AnyAsync(u => u.Email == req.Email))
            return Conflict(new { message = "E-mail já cadastrado." });

        var user = new User
        {
            Email = req.Email.ToLower().Trim(),
            Name = req.Name.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Region = req.Region
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(new AuthResponse(tokenService.GenerateToken(user), user.Id.ToString(), user.Name, user.Email));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email.ToLower().Trim());
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        return Ok(new AuthResponse(tokenService.GenerateToken(user), user.Id.ToString(), user.Name, user.Email));
    }
}
