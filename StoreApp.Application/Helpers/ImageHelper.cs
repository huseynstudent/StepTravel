 using Microsoft.AspNetCore.Http;

namespace StoreApp.Application.Helpers;


public static class ImageHelper
{
    // Call this from your handler — pass in webRootPath and the IFormFile
    public static async Task<(string imageUrl, string imageFileName)> SaveImageAsync(
        string webRootPath, IFormFile image, string baseUrl)
    {
        ArgumentNullException.ThrowIfNull(image);
        var uploadsFolder = Path.Combine(webRootPath, "images");
        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(image.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(stream);

        var imageUrl = $"{baseUrl}/images/{fileName}";
        return (imageUrl, fileName);
    }

    public static void DeleteImage(string webRootPath, string imageFileName)
    {
        if (string.IsNullOrEmpty(imageFileName)) return;
        var filePath = Path.Combine(webRootPath, "images", imageFileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}