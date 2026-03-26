using FilmForge.Api.Data;
using FilmForge.Api.Entities;
using FilmForge.Api.Models.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreateProfileController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public CreateProfileController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) ||
            string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.PrimaryCraft))
        {
            return BadRequest(new
            {
                Message = "UserId, FullName, Username, and PrimaryCraft are required."
            });
        }

        var usernameExists = await _dbContext.UserProfiles
            .AnyAsync(x => x.Username == request.Username);

        if (usernameExists)
        {
            return Conflict(new
            {
                Message = "That username is already taken."
            });
        }

        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            FullName = request.FullName,
            Username = request.Username,
            Bio = request.Bio,
            PrimaryCraft = request.PrimaryCraft,
            SecondaryCrafts = request.SecondaryCrafts,
            Location = request.Location,
            ExperienceLevel = request.ExperienceLevel,
            AvailabilityStatus = request.AvailabilityStatus,
            PortfolioUrl = request.PortfolioUrl,
            InstagramUrl = request.InstagramUrl,
            YoutubeUrl = request.YoutubeUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.UserProfiles.Add(profile);
        await _dbContext.SaveChangesAsync();

        var response = new CreateProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = profile.FullName,
            Username = profile.Username,
            Bio = profile.Bio,
            PrimaryCraft = profile.PrimaryCraft,
            SecondaryCrafts = profile.SecondaryCrafts,
            Location = profile.Location,
            ExperienceLevel = profile.ExperienceLevel,
            AvailabilityStatus = profile.AvailabilityStatus,
            PortfolioUrl = profile.PortfolioUrl,
            InstagramUrl = profile.InstagramUrl,
            YoutubeUrl = profile.YoutubeUrl,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };

        return Ok(response);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfileByUsername(string username)
    {
        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username);

        if (profile == null)
        {
            return NotFound(new
            {
                Message = "Profile not found."
            });
        }

        var response = new CreateProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = profile.FullName,
            Username = profile.Username,
            Bio = profile.Bio,
            PrimaryCraft = profile.PrimaryCraft,
            SecondaryCrafts = profile.SecondaryCrafts,
            Location = profile.Location,
            ExperienceLevel = profile.ExperienceLevel,
            AvailabilityStatus = profile.AvailabilityStatus,
            PortfolioUrl = profile.PortfolioUrl,
            InstagramUrl = profile.InstagramUrl,
            YoutubeUrl = profile.YoutubeUrl,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };

        return Ok(response);
    }

    [HttpGet("by-userid/{userId}")]
    public async Task<IActionResult> GetProfileByUserId(string userId)
    {
        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (profile == null)
        {
            return NotFound(new
            {
                Message = "Profile not found."
            });
        }

        var response = new CreateProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = profile.FullName,
            Username = profile.Username,
            Bio = profile.Bio,
            PrimaryCraft = profile.PrimaryCraft,
            SecondaryCrafts = profile.SecondaryCrafts,
            Location = profile.Location,
            ExperienceLevel = profile.ExperienceLevel,
            AvailabilityStatus = profile.AvailabilityStatus,
            PortfolioUrl = profile.PortfolioUrl,
            InstagramUrl = profile.InstagramUrl,
            YoutubeUrl = profile.YoutubeUrl,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };

        return Ok(response);
    }
}