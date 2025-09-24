using NewsletterService.Models;

namespace NewsletterService.Interfaces;

public interface ISubscriberRepository
{
    Task<Subscriber> AddSubscriberAsync(Subscriber subscriber);
    Task<List<Subscriber>> GetAllSubScribersAsync();
}