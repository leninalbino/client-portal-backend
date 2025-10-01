using ClientPortal.Application.DTOs;

namespace ClientPortal.Application.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync();
        Task<ClientDto?> GetClientByIdAsync(Guid id);
        Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto);
        Task<ClientDto> UpdateClientAsync(Guid id, UpdateClientDto updateClientDto);
        Task DeleteClientAsync(Guid id);
    }
}