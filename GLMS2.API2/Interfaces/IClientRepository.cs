using GLMS2.Models;

namespace GLMS2.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();

        Task<Client?> GetByIdAsync(int id);

        Task AddAsync(Client client);

        void Update(Client client);

        void Remove(Client client);

        Task SaveChangesAsync();
    }
}