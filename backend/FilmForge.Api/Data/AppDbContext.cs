using FilmForge.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmForge.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
     public DbSet<AppUser> AppUser => Set<AppUser>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostMedia> PostMedia => Set<PostMedia>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId).IsRequired().HasMaxLength(255);
            entity.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Username).IsRequired().HasMaxLength(80);
            entity.Property(x => x.PrimaryCraft).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Location).HasMaxLength(120);
            entity.Property(x => x.ExperienceLevel).HasMaxLength(50);
            entity.Property(x => x.AvailabilityStatus).HasMaxLength(50);
            entity.Property(x => x.PortfolioUrl).HasMaxLength(255);
            entity.Property(x => x.InstagramUrl).HasMaxLength(255);
            entity.Property(x => x.YoutubeUrl).HasMaxLength(255);

            entity.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("app_users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Username).IsRequired().HasMaxLength(80);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(150);
            entity.Property(x => x.PasswordHash).IsRequired();

            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
        });
        
        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("posts");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId).IsRequired().HasMaxLength(255);
            entity.Property(x => x.Caption).HasMaxLength(3000);

            entity.HasMany(x => x.MediaItems)
                .WithOne(x => x.Post)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Likes)
                .WithOne(x => x.Post)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Comments)
                .WithOne(x => x.Post)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PostMedia>(entity =>
        {
            entity.ToTable("post_media");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.MediaUrl).IsRequired().HasMaxLength(1000);
            entity.Property(x => x.MediaType).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.ToTable("post_likes");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId).IsRequired().HasMaxLength(255);

            entity.HasIndex(x => new { x.PostId, x.UserId }).IsUnique();
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.ToTable("post_comments");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId).IsRequired().HasMaxLength(255);
            entity.Property(x => x.Content).IsRequired().HasMaxLength(1500);
        });
    }
}