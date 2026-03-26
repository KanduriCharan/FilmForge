namespace FilmForge.Api.Models.Post;

public class PostResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PostMediaResponse> MediaItems { get; set; } = new();
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
}