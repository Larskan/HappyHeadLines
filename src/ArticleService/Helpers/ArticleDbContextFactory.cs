using ArticleService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ArticleService.Helpers
{
    public class ArticleDbContextFactory : IDesignTimeDbContextFactory<ArticleDbContext>
    {
        public ArticleDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ArticleDbContext>();
            optionsBuilder.UseSqlServer("Server=sqlserver,1435;Database=ArticleDb;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;");

            return new ArticleDbContext(optionsBuilder.Options);
        }
    }
}