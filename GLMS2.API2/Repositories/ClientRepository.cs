using GLMS2.Data;
using GLMS2.Interfaces;
using GLMS2.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == id);
        }
    }
}