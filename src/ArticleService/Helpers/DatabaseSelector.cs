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
            "NorthAmerica" => "Server=sqlserver,1433;Database=ArticleDB_NorthAmerica;User Id=sa;Password=My_Passw0rd123;",
            "SouthAmerica" => "Server=sqlserver,1433;Database=ArticleDB_SouthAmerica;User Id=sa;Password=My_Passw0rd123;",
            "Europe" => "Server=sqlserver,1433;Database=ArticleDB_Europe;User Id=sa;Password=My_Passw0rd123;",
            "Africa" => "Server=sqlserver,1433;Database=ArticleDB_Africa;User Id=sa;Password=My_Passw0rd123;",
            "Asia" => "Server=sqlserver,1433;Database=ArticleDB_Asia;User Id=sa;Password=My_Passw0rd123;",
            "Australia" => "Server=sqlserver,1433;Database=ArticleDB_Australia;User Id=sa;Password=My_Passw0rd123;",
            "Antarctica" => "Server=sqlserver,1433;Database=ArticleDB_Antarctica;User Id=sa;Password=My_Passw0rd123;",
            _ => "Server=sqlserver,1433;Database=ArticleDB_Global;User Id=sa;Password=My_Passw0rd123;"
        };
        optionsBuilder.UseSqlServer(connectionString);
        return new ArticleDbContext(optionsBuilder.Options);
    }
    
}