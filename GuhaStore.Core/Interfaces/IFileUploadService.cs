namespace GuhaStore.Core.Interfaces;

public interface IFileUploadService
{
    Task<string> UploadArticleImageAsync(Stream fileStream, string fileName);
    Task<string> UploadProductImageAsync(Stream fileStream, string fileName);
    Task<string> UploadCategoryImageAsync(Stream fileStream, string fileName);
    Task DeleteFileAsync(string filePath);
    Task<bool> ValidateImageFileAsync(Stream fileStream, string fileName, long fileSize);
}

