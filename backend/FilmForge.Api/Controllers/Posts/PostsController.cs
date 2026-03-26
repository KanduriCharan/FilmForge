using FilmForge.Api.Data;
using FilmForge.Api.Entities;
using FilmForge.Api.Models.Post;
using FilmForge.Api.Services.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IS3MediaService _s3MediaService;

    public PostsController(AppDbContext dbContext, IS3MediaService s3MediaService)
    {
        _dbContext = dbContext;
        _s3MediaService = s3MediaService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreatePost(
        [FromForm] string userId,
        [FromForm] string? caption,
        [FromForm] List<IFormFile>? files)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { Message = "UserId is required." });
        }

        files ??= new List<IFormFile>();

        if (string.IsNullOrWhiteSpace(caption) && files.Count == 0)
        {
            return BadRequest(new { Message = "A post needs a caption or media." });
        }

        if (files.Count > 5)
        {
            return BadRequest(new { Message = "You can upload up to 5 files only." });
        }

        var imageFiles = files.Where(f => f.ContentType.StartsWith("image/")).ToList();
        var videoFiles = files.Where(f => f.ContentType.StartsWith("video/")).ToList();

        if (imageFiles.Count > 0 && videoFiles.Count > 0)
        {
            return BadRequest(new { Message = "A post can contain either images or one video, not both." });
        }

        if (videoFiles.Count > 1)
        {
            return BadRequest(new { Message = "Only one video is allowed per post." });
        }

        var post = new Post
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Caption = caption ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Posts.Add(post);

        var mediaItems = new List<PostMedia>();

        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            var mediaType = file.ContentType.StartsWith("image/") ? "image" : "video";
            var mediaUrl = await _s3MediaService.UploadFileAsync(file, "posts");

            mediaItems.Add(new PostMedia
            {
                Id = Guid.NewGuid(),
                PostId = post.Id,
                MediaUrl = mediaUrl,
                MediaType = mediaType,
                SortOrder = i,
                CreatedAt = DateTime.UtcNow
            });
        }

        _dbContext.PostMedia.AddRange(mediaItems);
        await _dbContext.SaveChangesAsync();

        var response = new PostResponse
        {
            Id = post.Id,
            UserId = post.UserId,
            Caption = post.Caption,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            LikesCount = 0,
            CommentsCount = 0,
            MediaItems = mediaItems
                .OrderBy(x => x.SortOrder)
                .Select(x => new PostMediaResponse
                {
                    Id = x.Id,
                    MediaUrl = x.MediaUrl,
                    MediaType = x.MediaType,
                    SortOrder = x.SortOrder
                })
                .ToList()
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts()
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Include(x => x.MediaItems)
            .Include(x => x.Likes)
            .Include(x => x.Comments)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PostResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                Caption = x.Caption,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                LikesCount = x.Likes.Count,
                CommentsCount = x.Comments.Count,
                MediaItems = x.MediaItems
                    .OrderBy(m => m.SortOrder)
                    .Select(m => new PostMediaResponse
                    {
                        Id = m.Id,
                        MediaUrl = m.MediaUrl,
                        MediaType = m.MediaType,
                        SortOrder = m.SortOrder
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(posts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        var post = await _dbContext.Posts
            .AsNoTracking()
            .Include(x => x.MediaItems)
            .Include(x => x.Likes)
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (post == null)
        {
            return NotFound(new { Message = "Post not found." });
        }

        var response = new PostResponse
        {
            Id = post.Id,
            UserId = post.UserId,
            Caption = post.Caption,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            LikesCount = post.Likes.Count,
            CommentsCount = post.Comments.Count,
            MediaItems = post.MediaItems
                .OrderBy(m => m.SortOrder)
                .Select(m => new PostMediaResponse
                {
                    Id = m.Id,
                    MediaUrl = m.MediaUrl,
                    MediaType = m.MediaType,
                    SortOrder = m.SortOrder
                })
                .ToList()
        };

        return Ok(response);
    }

    [HttpPost("{postId:guid}/like")]
    public async Task<IActionResult> LikePost(Guid postId, [FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { Message = "UserId is required." });
        }

        var postExists = await _dbContext.Posts.AnyAsync(x => x.Id == postId);
        if (!postExists)
        {
            return NotFound(new { Message = "Post not found." });
        }

        var alreadyLiked = await _dbContext.PostLikes
            .AnyAsync(x => x.PostId == postId && x.UserId == userId);

        if (alreadyLiked)
        {
            return Conflict(new { Message = "Post already liked by this user." });
        }

        var like = new PostLike
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.PostLikes.Add(like);
        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Post liked successfully." });
    }

    [HttpDelete("{postId:guid}/like")]
    public async Task<IActionResult> UnlikePost(Guid postId, [FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { Message = "UserId is required." });
        }

        var like = await _dbContext.PostLikes
            .FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);

        if (like == null)
        {
            return NotFound(new { Message = "Like not found." });
        }

        _dbContext.PostLikes.Remove(like);
        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Post unliked successfully." });
    }

    [HttpPost("{postId:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid postId, [FromBody] CreateCommentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest(new { Message = "UserId and Content are required." });
        }

        var postExists = await _dbContext.Posts.AnyAsync(x => x.Id == postId);
        if (!postExists)
        {
            return NotFound(new { Message = "Post not found." });
        }

        var comment = new PostComment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = request.UserId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.PostComments.Add(comment);
        await _dbContext.SaveChangesAsync();

        return Ok(new PostCommentResponse
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        });
    }

    [HttpGet("{postId:guid}/comments")]
    public async Task<IActionResult> GetComments(Guid postId)
    {
        var postExists = await _dbContext.Posts.AnyAsync(x => x.Id == postId);
        if (!postExists)
        {
            return NotFound(new { Message = "Post not found." });
        }

        var comments = await _dbContext.PostComments
            .AsNoTracking()
            .Where(x => x.PostId == postId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PostCommentResponse
            {
                Id = x.Id,
                PostId = x.PostId,
                UserId = x.UserId,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        return Ok(comments);
    }
}