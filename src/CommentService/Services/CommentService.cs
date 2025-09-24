using CommentService.Models;
using CommentService.Repositories;
using CommentService.Interfaces;
using Shared;

namespace CommentService.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _repository;
    private readonly HttpClient _profanityClient;

    public CommentService(ICommentRepository repository, IHttpClientFactory httpClientFactory)
    {
        _repository = repository;
        _profanityClient = httpClientFactory.CreateClient("Profanity");
    }

    // Create a comment for a specific article with profanity filtering
    public async Task<CommentDto> CreateCommentAsync(Guid articleId, CommentDto commentDto)
    {
        string filteredText = commentDto.Body;
        try
        {
            var response = await _profanityClient.PostAsJsonAsync("/filter", new { text = commentDto.Body });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ProfanityDto>();
                filteredText = result!.Filtered;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] ProfanityService unavailable: {ex.Message}");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ArticleId = articleId,
            Author = commentDto.Author,
            Body = filteredText,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateCommentAsync(comment);
        return ToDto(created);
    }

    public async Task<List<CommentDto>> GetByArticleIdAsync(Guid articleId)
    {
        var comments = await _repository.GetByArticleIdAsync(articleId);
        return comments.Select(ToDto).ToList();
    }

    public async Task<bool> DeleteCommentAsync(Guid id) =>
        await _repository.DeleteCommentAsync(id);

    // Mapping method from Comment to CommentDto
    private static CommentDto ToDto(Comment c) =>
        new CommentDto(c.Id, c.ArticleId, c.Body, c.Author, c.CreatedAt);
}