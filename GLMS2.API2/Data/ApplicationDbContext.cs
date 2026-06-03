using GLMS2.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor receives configuration from Program.cs
        // Allows SQL Server connection to be injected at runtime
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // Represents Clients table in SQL Server
        // Stores client information used to link contracts
        public DbSet<Client> Clients { get; set; }
        // Represents Contracts table
        // Each contract is linked to a client
        public DbSet<Contract> Contracts { get; set; }
        // Represents ServiceRequests table
        // Each service request must belong to a contract
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Contracts)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contract>()
                .HasMany(c => c.ServiceRequests)
                .WithOne(sr => sr.Contract)
                .HasForeignKey(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
            // Configures currency values with precision suitable for financial calculations
            // Prevents rounding errors when storing USD amounts
            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.CostUSD)
                .HasPrecision(18, 2);
            // Stores converted ZAR values with consistent decimal precision
            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.CostZAR)
                .HasPrecision(18, 2);


           
        }
    }
}