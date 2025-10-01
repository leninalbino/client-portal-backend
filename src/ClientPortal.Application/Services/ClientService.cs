using ClientPortal.Application.DTOs;
using ClientPortal.Domain.Entities;
using ClientPortal.Domain.Interfaces;

namespace ClientPortal.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IFileService _fileService;

        public ClientService(IClientRepository clientRepository, IFileService fileService)
        {
            _clientRepository = clientRepository;
            _fileService = fileService;
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            var clients = await _clientRepository.GetAllAsync();
            return clients.Select(MapToDto);
        }

        public async Task<ClientDto?> GetClientByIdAsync(Guid id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return client != null ? MapToDto(client) : null;
        }

        public async Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto)
        {
            // Verificar que no exista otro cliente con el mismo documento
            if (await _clientRepository.ExistsAsync(createClientDto.DocumentNumber, createClientDto.DocumentType))
            {
                throw new InvalidOperationException("Ya existe un cliente con ese documento");
            }

            // Validar que los archivos estén bien
            ValidateFiles(createClientDto.CurriculumVitae, createClientDto.Photo);

            var cvFile = await _fileService.SaveFileAsync(
                createClientDto.CurriculumVitae.OpenReadStream(),
                createClientDto.CurriculumVitae.FileName,
                "cv");

            var photoFile = await _fileService.SaveFileAsync(
                createClientDto.Photo.OpenReadStream(),
                createClientDto.Photo.FileName,
                "photo");

            var client = new Client
            {
                Id = Guid.NewGuid(),
                FirstName = createClientDto.FirstName,
                LastName = createClientDto.LastName,
                DateOfBirth = createClientDto.DateOfBirth,
                DocumentType = createClientDto.DocumentType,
                DocumentNumber = createClientDto.DocumentNumber,
                CurriculumVitaeFileName = cvFile.fileName,
                CurriculumVitaeFilePath = cvFile.filePath,
                PhotoFileName = photoFile.fileName,
                PhotoFilePath = photoFile.filePath,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var savedClient = await _clientRepository.AddAsync(client);
            return MapToDto(savedClient);
        }

        public async Task<ClientDto> UpdateClientAsync(Guid id, UpdateClientDto updateClientDto)
        {
            // Buscar el cliente que queremos actualizar
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null || client.IsDeleted)
                throw new KeyNotFoundException("No se encontró el cliente para actualizar");

            client.FirstName = updateClientDto.FirstName;
            client.LastName = updateClientDto.LastName;
            client.DateOfBirth = updateClientDto.DateOfBirth;

            if (updateClientDto.CurriculumVitae != null)
            {
                ValidateFile(updateClientDto.CurriculumVitae, "pdf", 5);
                
                if (!string.IsNullOrEmpty(client.CurriculumVitaeFilePath))
                    await _fileService.DeleteFileAsync(client.CurriculumVitaeFilePath);

                var cvFile = await _fileService.SaveFileAsync(
                    updateClientDto.CurriculumVitae.OpenReadStream(),
                    updateClientDto.CurriculumVitae.FileName,
                    "cv");

                client.CurriculumVitaeFileName = cvFile.fileName;
                client.CurriculumVitaeFilePath = cvFile.filePath;
            }

            if (updateClientDto.Photo != null)
            {
                ValidateFile(updateClientDto.Photo, "jpeg", 5);
                
                if (!string.IsNullOrEmpty(client.PhotoFilePath))
                    await _fileService.DeleteFileAsync(client.PhotoFilePath);

                var photoFile = await _fileService.SaveFileAsync(
                    updateClientDto.Photo.OpenReadStream(),
                    updateClientDto.Photo.FileName,
                    "photo");

                client.PhotoFileName = photoFile.fileName;
                client.PhotoFilePath = photoFile.filePath;
            }

            client.UpdatedAt = DateTime.UtcNow;

            var updatedClient = await _clientRepository.UpdateAsync(client);
            return MapToDto(updatedClient);
        }

        public async Task DeleteClientAsync(Guid id)
        {
            await _clientRepository.DeleteAsync(id);
        }

        private static ClientDto MapToDto(Client client)
        {
            return new ClientDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                DateOfBirth = client.DateOfBirth,
                DocumentType = client.DocumentType,
                DocumentNumber = client.DocumentNumber,
                CurriculumVitaeFileName = client.CurriculumVitaeFileName,
                PhotoFileName = client.PhotoFileName,
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt
            };
        }

        private static void ValidateFiles(Microsoft.AspNetCore.Http.IFormFile cv, Microsoft.AspNetCore.Http.IFormFile photo)
        {
            ValidateFile(cv, "pdf", 5);
            ValidateFile(photo, "jpeg", 5);
        }

        private static void ValidateFile(Microsoft.AspNetCore.Http.IFormFile file, string expectedFormat, int maxSizeMB)
        {
            // Verificar tamaño del archivo
            if (file.Length > maxSizeMB * 1024 * 1024)
                throw new InvalidOperationException($"El archivo no puede pesar más de {maxSizeMB}MB");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (expectedFormat == "pdf" && extension != ".pdf")
                throw new InvalidOperationException("El CV debe ser un archivo PDF");

            if (expectedFormat == "jpeg" && !new[] { ".jpg", ".jpeg" }.Contains(extension))
                throw new InvalidOperationException("La foto debe ser JPG o JPEG");
        }
    }
}