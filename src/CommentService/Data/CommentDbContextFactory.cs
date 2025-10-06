using CommentService.Data;
using CommentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CommentService.Helpers
{
    // This class is used by EF Core to create migrations and update the database at design time.
    // It provides a way to configure the DbContext with the correct connection string.
    // Prevents: An error occurred while accessing the Microsoft.Extensions.Hosting services. Continuing without the application service provider. Error: Some services are not able to be constructed (Error while validating the service descriptor 'ServiceType: CommentService.Data.CommentCache Lifetime: Singleton ImplementationType: CommentService.Data.CommentCache': Unable to resolve service for type 'Shared.IRedisHelper' while attempting to activate 'CommentService.Data.CommentCache'.)
    public class CommentDbContextFactory : IDesignTimeDbContextFactory<CommentDbContext>
    {
        public CommentDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommentDbContext>();
            optionsBuilder.UseSqlServer("Server=sqlserver,1435;Database=CommentDb;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;");

            return new CommentDbContext(optionsBuilder.Options);
        }
    }
}