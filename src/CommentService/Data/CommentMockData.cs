using Moq;
using CommentService.Interfaces;
using CommentService.Models;
using System.Diagnostics;

namespace CommentService.Data;

public class CommentMockData
{
    public ICommentRepository commentRepository => mockCommentRepo.Object;
    private Mock<ICommentRepository> mockCommentRepo;

    public CommentMockData()
    {
        Debug.WriteLine("CommentMockData constructor called");

        // Test data for comments
        var comments = new List<Comment>
        {
            new Comment{Id = 1, ArticleId = 1, Body = "First comment on article 1", CreatedAt = DateTime.UtcNow.AddDays(-2)},
            new Comment{Id = 2, ArticleId = 1, Body = "Second comment on article 1", CreatedAt = DateTime.UtcNow.AddDays(-1)},
            new Comment{Id = 3, ArticleId = 2, Body = "First comment on article 2", CreatedAt = DateTime.UtcNow.AddDays(-12)},
            new Comment{Id = 4, ArticleId = 3, Body = "First comment on article 3", CreatedAt = DateTime.UtcNow.AddDays(-6)},
        };

        // Mock repo
        mockCommentRepo = new Mock<ICommentRepository>();

        // Get comment by commentId
        mockCommentRepo.Setup(repo => repo.GetByCommentIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => comments.Find(c => c.Id == id));

        // Get comments by articleId
        mockCommentRepo.Setup(repo => repo.GetByArticleIdAsync(It.IsAny<int>())).ReturnsAsync((int articleId) => comments.FindAll(c => c.ArticleId == articleId));

        // Create comment
        mockCommentRepo.Setup(repo => repo.CreateCommentAsync(It.IsAny<Comment>())).ReturnsAsync((Comment comment) =>
        {
            int newId = comments.Count + 1;
            comment.Id = newId;
            comment.CreatedAt = DateTime.UtcNow;
            comments.Add(comment);
            return comment;
        });

        //Delete comment
        mockCommentRepo.Setup(repo => repo.DeleteCommentAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
        {
            var comment = comments.Find(c => c.Id == id);
            if (comment == null) return false;

            comments.Remove(comment);
            return true;
        });
    }
}

