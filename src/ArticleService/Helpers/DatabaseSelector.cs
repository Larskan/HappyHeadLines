using ArticleService.Helpers;
using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Helpers;

public static class DatabaseSelector
{
    public static ArticleDbContext GetDbContext(IServiceProvider services, string continent)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ArticleDbContext>();
        string connectionString = continent switch
        {
            "NorthAmerica" => "Server=sqlserver,1433;Database=ArticleDb_NorthAmerica;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;",
            "SouthAmerica" => "Server=sqlserver,1433;Database=ArticleDb_SouthAmerica;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;",
            "Europe" => "Server=sqlserver,1433;Database=ArticleDb_Europe;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;",
            "Africa" => "Server=sqlserver,1433;Database=ArticleDb_Africa;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;",
            "Asia" => "Server=sqlserver,1433;Database=ArticleDb_Asia;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;",
            "Australia" => "Server=sqlserver,1433;Database=ArticleDb_Australia;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;",
            "Antarctica" => "Server=sqlserver,1433;Database=ArticleDb_Antarctica;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;",
            _ => "Server=sqlserver,1433;Database=ArticleDb_Global;User Id=sa;Password=My_Passw0rd123;TrustServerCertificate=True;"
        };
        optionsBuilder.UseSqlServer(connectionString);
        return new ArticleDbContext(optionsBuilder.Options);
    }
    
}