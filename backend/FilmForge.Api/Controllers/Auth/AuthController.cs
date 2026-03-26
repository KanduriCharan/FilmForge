using BCrypt.Net;
using FilmForge.Api.Data;
using FilmForge.Api.Entities;
using FilmForge.Api.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public AuthController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                Message = "All signup fields are required."
            });
        }

        var emailExists = await _dbContext.AppUser
            .AnyAsync(x => x.Email == request.Email);

        if (emailExists)
        {
            return Conflict(new
            {
                Message = "An account with this email already exists."
            });
        }

        var usernameExists = await _dbContext.AppUser
            .AnyAsync(x => x.Username == request.Username);

        if (usernameExists)
        {
            return Conflict(new
            {
                Message = "That username is already taken."
            });
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.AppUser.Add(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new AuthResponse
        {
            Message = "Signup successful.",
            UserId = user.Id.ToString(),
            Username = user.Username,
            Email = user.Email
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                Message = "Email and password are required."
            });
        }

        var user = await _dbContext.AppUser
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            return Unauthorized(new
            {
                Message = "Invalid email or password."
            });
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
        {
            return Unauthorized(new
            {
                Message = "Invalid email or password."
            });
        }

        return Ok(new AuthResponse
        {
            Message = "Login successful.",
            UserId = user.Id.ToString(),
            Username = user.Username,
            Email = user.Email
        });
    }
}