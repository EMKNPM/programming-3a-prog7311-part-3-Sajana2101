using GLMS2.Data;
using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .OrderByDescending(c => c.ContractId)
                .ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.ContractId == id);
        }

        public async Task<Contract?> GetForUpdateAsync(int id)
        {
            return await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == id);
        }

        public async Task<IEnumerable<Contract>> FilterAsync(
            DateTime? startDateFrom,
            DateTime? startDateTo,
            ContractStatus? status)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (startDateFrom.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDateFrom.Value);
            }

            if (startDateTo.HasValue)
            {
                query = query.Where(c => c.StartDate <= startDateTo.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            return await query
                .OrderByDescending(c => c.ContractId)
                .ToListAsync();
        }

        public async Task AddAsync(Contract contract)
        {
            await _context.Contracts.AddAsync(contract);
        }

        public void Update(Contract contract)
        {
            _context.Contracts.Update(contract);
        }

        public void Remove(Contract contract)
        {
            _context.Contracts.Remove(contract);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}