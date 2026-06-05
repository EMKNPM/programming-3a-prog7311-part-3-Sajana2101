using GLMS2.Models;

namespace GLMS2.Interfaces
{
    public interface IClientApiService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();

        Task<Client?> GetClientByIdAsync(int id);

        Task CreateClientAsync(Client client);

        Task<bool> UpdateClientAsync(Client client);

        Task<bool> DeleteClientAsync(int id);
    }
}