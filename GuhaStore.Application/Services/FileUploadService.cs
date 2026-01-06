using GuhaStore.Core.Interfaces;

namespace GuhaStore.Application.Services;

public class FileUploadService : IFileUploadService
{
    private readonly string _webRootPath;
    private const int MaxFileSize = 5 * 1024 * 1024; // 5MB
    private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public FileUploadService(string webRootPath)
    {
        _webRootPath = webRootPath;
    }

    public async Task<string> UploadArticleImageAsync(Stream fileStream, string fileName)
    {
        if (!await ValidateImageFileAsync(fileStream, fileName, fileStream.Length))
            throw new ArgumentException("Invalid image file");

        var safeFileName = $"article_{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(fileName)}";
        var uploadPath = Path.Combine(_webRootPath, "uploads", "articles");
        
        Directory.CreateDirectory(uploadPath);
        
        var filePath = Path.Combine(uploadPath, safeFileName);
        using (var outputStream = new FileStream(filePath, FileMode.Create))
        {
            fileStream.Position = 0;
            await fileStream.CopyToAsync(outputStream);
        }

        return $"/uploads/articles/{safeFileName}";
    }

    public async Task<string> UploadProductImageAsync(Stream fileStream, string fileName)
    {
        if (!await ValidateImageFileAsync(fileStream, fileName, fileStream.Length))
            throw new ArgumentException("Invalid image file");

        var safeFileName = $"product_{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(fileName)}";
        var uploadPath = Path.Combine(_webRootPath, "uploads", "products");
        
        Directory.CreateDirectory(uploadPath);
        
        var filePath = Path.Combine(uploadPath, safeFileName);
        using (var outputStream = new FileStream(filePath, FileMode.Create))
        {
            fileStream.Position = 0;
            await fileStream.CopyToAsync(outputStream);
        }

        return $"/uploads/products/{safeFileName}";
    }

    public async Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        var fullPath = Path.Combine(_webRootPath, filePath.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            await Task.Run(() => File.Delete(fullPath));
        }
    }

    public async Task<bool> ValidateImageFileAsync(Stream fileStream, string fileName, long fileSize)
    {
        if (fileStream == null || fileSize == 0)
            return false;

        if (fileSize > MaxFileSize)
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return false;

        return await Task.FromResult(true);
    }
}

