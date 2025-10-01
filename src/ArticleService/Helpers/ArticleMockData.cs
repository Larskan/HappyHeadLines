using ArticleService.Interfaces;
using ArticleService.Models;
using Moq;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArticleService.Helpers;

public class ArticleMockData
{
    public IArticleRepository articleRepository => mockArticleRepo.Object;
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
        mockArticleRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), "Global")).ReturnsAsync((int id, string continent) => articles.Find(a => a.Id == id));

        // Create article
        mockArticleRepo.Setup(repo => repo.CreateArticleAsync(It.IsAny<Article>(), "Global")).ReturnsAsync((Article art, string continent) =>
        {
            int newId = articles.Count + 1;
            art.Id = newId;
            articles.Add(art);
            return art;
        });

        // Update article
        mockArticleRepo.Setup(repo => repo.UpdateArticleAsync(It.IsAny<Article>(), "Global")).ReturnsAsync((Article art, string continent) =>
        {
            var existing = articles.Find(a => a.Id == art.Id);
            if (existing == null) return false;

            existing.Title = art.Title;
            existing.Body = art.Body;
            return true;
        });

        // Delete article
        mockArticleRepo.Setup(repo => repo.DeleteArticleAsync(It.IsAny<int>(), "Global")).ReturnsAsync((int id, string continent) =>
        {
            var art = articles.Find(a => a.Id == id);
            if (art == null) return false;
            articles.Remove(art);
            return true;
        });

        // Get articles since a specific date
        mockArticleRepo.Setup(repo => repo.GetArticlesSinceAsync(It.IsAny<DateTime>())).ReturnsAsync((DateTime since) => articles.FindAll(a => a.CreatedAt >= since));
    }

}