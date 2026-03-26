namespace FilmForge.Api.Entities;

public class Post
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<PostMedia> MediaItems { get; set; } = new();
    public List<PostLike> Likes { get; set; } = new();
    public List<PostComment> Comments { get; set; } = new();
}