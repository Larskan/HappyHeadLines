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
            "NorthAmerica" => "Server=sqlserver,1433;Database=ArticleDb_NorthAmerica;User Id=sa;Password=My_Passw0rd123;",
            "SouthAmerica" => "Server=sqlserver,1433;Database=ArticleDb_SouthAmerica;User Id=sa;Password=My_Passw0rd123;",
            "Europe" => "Server=sqlserver,1433;Database=ArticleDb_Europe;User Id=sa;Password=My_Passw0rd123;",
            "Africa" => "Server=sqlserver,1433;Database=ArticleDb_Africa;User Id=sa;Password=My_Passw0rd123;",
            "Asia" => "Server=sqlserver,1433;Database=ArticleDb_Asia;User Id=sa;Password=My_Passw0rd123;",
            "Australia" => "Server=sqlserver,1433;Database=ArticleDb_Australia;User Id=sa;Password=My_Passw0rd123;",
            "Antarctica" => "Server=sqlserver,1433;Database=ArticleDb_Antarctica;User Id=sa;Password=My_Passw0rd123;",
            _ => "Server=sqlserver,1433;Database=ArticleDb_Global;User Id=sa;Password=My_Passw0rd123;"
        };
        optionsBuilder.UseSqlServer(connectionString);
        return new ArticleDbContext(optionsBuilder.Options);
    }
    
}