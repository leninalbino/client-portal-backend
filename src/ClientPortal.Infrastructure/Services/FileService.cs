using ClientPortal.Domain.Interfaces;

namespace ClientPortal.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadsPath;

        public FileService()
        {
            _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(_uploadsPath))
                Directory.CreateDirectory(_uploadsPath);
        }

        public async Task<(string fileName, string filePath)> SaveFileAsync(Stream fileStream, string originalFileName, string fileType)
        {
            var fileExtension = Path.GetExtension(originalFileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var typeFolder = Path.Combine(_uploadsPath, fileType);
            
            if (!Directory.Exists(typeFolder))
                Directory.CreateDirectory(typeFolder);

            var filePath = Path.Combine(typeFolder, uniqueFileName);

            using (var fileStreamDestination = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamDestination);
            }

            return (uniqueFileName, filePath);
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (await FileExistsAsync(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            return await Task.FromResult(File.Exists(filePath));
        }
    }
}