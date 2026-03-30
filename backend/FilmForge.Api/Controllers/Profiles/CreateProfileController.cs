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

        var existingUserProfile = await _dbContext.UserProfiles
            .AnyAsync(x => x.UserId == request.UserId);

        if (existingUserProfile)
        {
            return Conflict(new
            {
                Message = "A profile already exists for this user."
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
            ProfileImageUrl = request.ProfileImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.UserProfiles.Add(profile);
        await _dbContext.SaveChangesAsync();

        return Ok(MapProfile(profile));
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateProfile(string userId, [FromBody] CreateProfileRequest request)
    {
        var profile = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (profile == null)
        {
            return NotFound(new { Message = "Profile not found." });
        }

        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.PrimaryCraft))
        {
            return BadRequest(new
            {
                Message = "FullName, Username, and PrimaryCraft are required."
            });
        }

        var usernameTakenByAnotherUser = await _dbContext.UserProfiles
            .AnyAsync(x => x.Username == request.Username && x.UserId != userId);

        if (usernameTakenByAnotherUser)
        {
            return Conflict(new { Message = "That username is already taken." });
        }

        profile.FullName = request.FullName;
        profile.Username = request.Username;
        profile.Bio = request.Bio;
        profile.PrimaryCraft = request.PrimaryCraft;
        profile.SecondaryCrafts = request.SecondaryCrafts;
        profile.Location = request.Location;
        profile.ExperienceLevel = request.ExperienceLevel;
        profile.AvailabilityStatus = request.AvailabilityStatus;
        profile.PortfolioUrl = request.PortfolioUrl;
        profile.InstagramUrl = request.InstagramUrl;
        profile.YoutubeUrl = request.YoutubeUrl;

        if (!string.IsNullOrWhiteSpace(request.ProfileImageUrl))
        {
            profile.ProfileImageUrl = request.ProfileImageUrl;
        }

        profile.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(MapProfile(profile));
    }

    [HttpPut("{userId}/image")]
    public async Task<IActionResult> UpdateProfileImage(string userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Message = "Profile image file is required." });
        }

        var profile = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (profile == null)
        {
            return NotFound(new { Message = "Profile not found." });
        }

        // TEMPORARY:
        // Replace this later with real S3 upload using your media service.
        // For now, store a placeholder value or wire actual S3 upload here.
        var fakeUploadedUrl = $"https://your-cdn-or-s3-url/profile-images/{Guid.NewGuid()}-{file.FileName}";

        profile.ProfileImageUrl = fakeUploadedUrl;
        profile.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(MapProfile(profile));
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfileByUsername(string username)
    {
        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username);

        if (profile == null)
        {
            return NotFound(new { Message = "Profile not found." });
        }

        return Ok(MapProfile(profile));
    }

    [HttpGet("by-userid/{userId}")]
    public async Task<IActionResult> GetProfileByUserId(string userId)
    {
        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (profile == null)
        {
            return NotFound(new { Message = "Profile not found." });
        }

        return Ok(MapProfile(profile));
    }

    private static CreateProfileResponse MapProfile(UserProfile profile)
    {
        return new CreateProfileResponse
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
            ProfileImageUrl = profile.ProfileImageUrl,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }
}