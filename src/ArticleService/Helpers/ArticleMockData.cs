using ArticleService.Interfaces;
using ArticleService.Models;
using Moq;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArticleService.Helpers;

public class ArticleMockData
{
    public IArticleRepository articleRepository => mockArticleRepo.Object;
    public IArticleService articleService => articleService;
    private Mock<IArticleRepository> mockArticleRepo;

    public ArticleMockData()
    {
        Debug.WriteLine("ArticleMockData constructor called");

        // Test articles.
        var articles = new List<Article>
        {
            new Article{Id = 1, Title = "Test Article 1", Body = "Bla bla"},
            new Article{Id = 2, Title = "Test Article 2", Body = "Bla bla bla"},
            new Article{Id = 3, Title = "Test Article 3", Body = "Bla bla bla bla"},
        };

        // Mock repo
        mockArticleRepo = new Mock<IArticleRepository>();

        // Setup GetAllAsync() to return test articles
        mockArticleRepo.Setup(repo => repo.GetAllAsync("Global")).ReturnsAsync(articles);

        // Setup GetByIdAsync to return single article
        
    }

}