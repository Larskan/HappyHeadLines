namespace Shared;

public record ArticleDto(int Id, string Title, string Body, DateTime PublishedAt, string Author);
public record CommentDto(int Id, int ArticleId, string Author, string Body, DateTime PostedAt);
public record ProfanityDto(int Id, string Filtered);
public record DraftDto(int Id, int AuthorId, string Title, string Body, DateTime CreatedAt, DateTime UpdatedAt);
public record DraftCreateDto(string Title, string Body, int AuthorId);
public record DraftUpdateDto(string Title, string Body);
public record ArticleMessageDto(int Id, int AuthorId, string Title, string Body, DateTime PublishedAt);
public record SubscriberDto(int Id, string Email, string? Name);