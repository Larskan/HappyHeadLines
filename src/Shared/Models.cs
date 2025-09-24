namespace Shared;

public record ArticleDto(Guid Id, string Title, string Body, DateTime PublishedAt, string Author);
public record CommentDto(Guid Id, Guid ArticleId, string Author, string Body, DateTime PostedAt);
public record SubscriberDto(Guid Id, string Email, bool Active, DateTime SubscribedAt);
public record ProfanityDto(string Filtered);