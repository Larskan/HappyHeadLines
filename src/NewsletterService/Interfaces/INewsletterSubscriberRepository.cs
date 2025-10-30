using NewsletterService.Models;
using Shared;

namespace NewsletterService.Interfaces;

public interface INewsletterSubscriberRepository
{
    Task<List<SubscriberDto>> GetAllSubscribersAsync(CancellationToken ct = default);
}