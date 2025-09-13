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
            "NorthAmerica" => "Server=localhost;Database=ArticleDB_NorthAmerica;User Id=sa;Password=My_Passw0rd123;",
            "SouthAmerica" => "Server=localhost;Database=ArticleDB_SouthAmerica;User Id=sa;Password=My_Passw0rd123;",
            "Europe" => "Server=localhost;Database=ArticleDB_Europe;User Id=sa;Password=My_Passw0rd123;",
            "Africa" => "Server=localhost;Database=ArticleDB_Africa;User Id=sa;Password=My_Passw0rd123;",
            "Asia" => "Server=localhost;Database=ArticleDB_Asia;User Id=sa;Password=My_Passw0rd123;",
            "Australia" => "Server=localhost;Database=ArticleDB_Australia;User Id=sa;Password=My_Passw0rd123;",
            "Antarctica" => "Server=localhost;Database=ArticleDB_Antarctica;User Id=sa;Password=My_Passw0rd123;",
            _ => "Server=localhost;Database=ArticleDB_Global;User Id=sa;Password=My_Passw0rd123;"
        };
        optionsBuilder.UseSqlServer(connectionString);
        return new ArticleDbContext(optionsBuilder.Options);
    }
    
}