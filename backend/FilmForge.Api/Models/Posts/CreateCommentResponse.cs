namespace FilmForge.Api.Models.Post;

public class CreateCommentRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}