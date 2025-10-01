namespace ClientPortal.Domain.Interfaces
{
    public interface IFileService
    {
        Task<(string fileName, string filePath)> SaveFileAsync(Stream fileStream, string originalFileName, string fileType);
        Task DeleteFileAsync(string filePath);
        Task<bool> FileExistsAsync(string filePath);
    }
}