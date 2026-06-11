namespace TomeApp.API.Models.DTOs;

public record RegisterRequest(string Email, string Name, string Password, string Region = "BR");
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string UserId, string Name, string Email);

public record UpdateProfileRequest(string? Name, string? Bio, string? AvatarUrl);
