using SubscriberService.Models;

namespace SubscriberService.Interfaces;

public interface ISubscriberRepository
{
    Task<Subscriber> AddSubscriberAsync(Subscriber subscriber);
    Task<List<Subscriber>> GetAllSubScribersAsync();
}