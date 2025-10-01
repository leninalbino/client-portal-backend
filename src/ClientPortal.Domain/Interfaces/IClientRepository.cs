using ClientPortal.Domain.Entities;

namespace ClientPortal.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(Guid id);
        Task<Client> AddAsync(Client client);
        Task<Client> UpdateAsync(Client client);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(string documentNumber, DocumentType documentType);
    }
}