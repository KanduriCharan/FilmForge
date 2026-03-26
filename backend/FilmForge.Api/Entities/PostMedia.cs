namespace FilmForge.Api.Entities;

public class PostMedia
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty; 
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }

    public Post Post { get; set; } = null!;
}