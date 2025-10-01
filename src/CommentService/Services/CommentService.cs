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
    public async Task<CommentDto> CreateCommentAsync(int articleId, CommentDto commentDto)
    {
        string filteredText = commentDto.Body;
        try
        {
            var response = await _profanityClient.PostAsJsonAsync("/filter", new { text = commentDto.Body });
            // If the ProfanityService is available and returns success, use the filtered text
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ProfanityDto>();
                filteredText = result!.Filtered;
            }
        }
        catch (Exception ex)
        {
            // If ProfanityService is down or fails, log the error and proceed with unfiltered text
            Console.WriteLine($"[WARN] ProfanityService unavailable: {ex.Message}");
        }

        var comment = new Comment
        {
            Id = commentDto.Id,
            ArticleId = articleId,
            Author = commentDto.Author,
            Body = filteredText,
            IsFiltered = filteredText != commentDto.Body, // False if profanity check failed
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateCommentAsync(comment);
        return ToDto(created);
    }

    public async Task<List<CommentDto>> GetByArticleIdAsync(int articleId)
    {
        var comments = await _repository.GetByArticleIdAsync(articleId);
        return comments.Select(ToDto).ToList();
    }

    public async Task<bool> DeleteCommentAsync(int id) =>
        await _repository.DeleteCommentAsync(id);

    // Mapping method from Comment to CommentDto
    private static CommentDto ToDto(Comment c) =>
        new CommentDto(c.Id, c.ArticleId, c.Body, c.Author, c.CreatedAt);
}