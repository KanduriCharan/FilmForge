namespace FilmForge.Api.Models.Post;

public class PostMediaResponse
{
    public Guid Id { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}