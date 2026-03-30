namespace FilmForge.Api.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string PrimaryCraft { get; set; } = string.Empty;
    public string? SecondaryCrafts { get; set; }
    public string? Location { get; set; }
    public string? ExperienceLevel { get; set; }
    public string? AvailabilityStatus { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? ProfileImageUrl { get; set; }
}