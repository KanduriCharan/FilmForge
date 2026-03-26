namespace FilmForge.Api.Entities;

public class PostLike
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Post Post { get; set; } = null!;
}