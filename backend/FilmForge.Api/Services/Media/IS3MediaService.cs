using Microsoft.AspNetCore.Http;

namespace FilmForge.Api.Services.Media;

public interface IS3MediaService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName);
    string GeneratePresignedGetUrl(string objectKey, int expiryMinutes = 60);
    string ExtractObjectKeyFromUrl(string mediaUrl);
}