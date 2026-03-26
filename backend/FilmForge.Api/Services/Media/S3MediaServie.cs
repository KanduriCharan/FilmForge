using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace FilmForge.Api.Services.Media;

public class S3MediaService : IS3MediaService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;

    public S3MediaService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _configuration = configuration;
    }

    public string ExtractObjectKeyFromUrl(string mediaUrl)
    {
        throw new NotImplementedException();
    }

    public string GeneratePresignedGetUrl(string objectKey, int expiryMinutes = 60)
    {
        throw new NotImplementedException();
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        var bucketName = _configuration["AWS:S3BucketName"];
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new InvalidOperationException("S3 bucket name is not configured.");
        }

        var extension = Path.GetExtension(file.FileName);
        var key = $"{folderName}/{Guid.NewGuid()}{extension}";

        using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3Client.PutObjectAsync(request);

        var cloudFrontDomain = _configuration["AWS:CloudFrontDomain"];
        if (string.IsNullOrWhiteSpace(cloudFrontDomain))
        {
            throw new InvalidOperationException("CloudFront domain is not configured.");
        }

        return $"https://{cloudFrontDomain}/{key}";
    }
}