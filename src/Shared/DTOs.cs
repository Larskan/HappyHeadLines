namespace Shared;

public record ArticleDto(Guid Id, string Title, string Body, DateTime PublishedAt, string Author);
public record CommentDto(Guid Id, Guid ArticleId, string Author, string Body, DateTime PostedAt);
public record SubscriberDto(Guid Id, string Email, bool Active, DateTime SubscribedAt);
public record ProfanityDto(Guid Id, string Filtered);
public record DraftDto(Guid Id, string Author, string Title, string Body, DateTime CreatedAt, DateTime UpdatedAt);
public record DraftCreateDto(string Title, string Body, string Author);
public record DraftUpdateDto(string Title, string Body);