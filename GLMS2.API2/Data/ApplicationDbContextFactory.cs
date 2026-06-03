using GLMS2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GLMS2.Data
{
    // Provides a design-time DbContext for EF Core tools
    // Ensures migrations can be created even when the application is not running
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Configures SQL Server connection for the GLMS database
            // Supports Entity Framework Core migrations and database updates
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=GLMSDB;Trusted_Connection=True;TrustServerCertificate=True;");
            // Returns configured DbContext used by EF Core tools
            // Allows creation of tables for Client, Contract, and ServiceRequest entities
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}