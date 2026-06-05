using GLMS2.Models;

namespace GLMS2.Interfaces
{
    public interface IClientApiService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
}