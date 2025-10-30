using SubscriberService.Models;

namespace SubscriberService.Interfaces;

public interface ISubscriberRepository
{
    Task<Subscriber> AddSubscriberAsync(Subscriber subscriber);
    Task<bool> DeleteSubscriberAsync(int id, CancellationToken ct = default);
    Task<List<Subscriber>> GetAllSubScribersAsync();
}