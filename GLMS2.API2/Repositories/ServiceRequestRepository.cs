using GLMS2.Data;
using GLMS2.Interfaces;
using GLMS2.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .OrderByDescending(sr => sr.ServiceRequestId)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);
        }

        public async Task<ServiceRequest?> GetForUpdateAsync(int id)
        {
            return await _context.ServiceRequests
                .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);
        }

        public async Task AddAsync(ServiceRequest serviceRequest)
        {
            await _context.ServiceRequests.AddAsync(serviceRequest);
        }

        public void Update(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);
        }

        public void Remove(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Remove(serviceRequest);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}