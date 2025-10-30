using Shared;
using SubscriberService.Models;

namespace SubscriberService.Interfaces;

public interface ISubcriberService
{
    Task<Subscriber> AddSubscriberAsync(Subscriber subscriber, CancellationToken ct = default);
    Task<bool> RemoveSubscriberAsync(int id, CancellationToken ct = default);
    Task<List<SubscriberDto>> GetAllAsync(CancellationToken ct = default);
}