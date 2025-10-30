using SubscriberService.Interfaces;
using SubscriberService.Models;
using Shared;
using SubscriberService.Repositories;

namespace SubscriberService.Services;

public class SubscriberService : ISubcriberService
{
    private readonly ISubscriberRepository _repo;
    private readonly ISubscriberQueuePublisher _subscriberQueue;

    public SubscriberService(ISubscriberRepository repo, ISubscriberQueuePublisher subscriberQueue)
    {
        _repo = repo;
        _subscriberQueue = subscriberQueue;
    }

    public async Task<Subscriber> AddSubscriberAsync(Subscriber subscriber, CancellationToken ct = default)
    {
        var created = await _repo.AddSubscriberAsync(subscriber);
        //publish to queue for other swimlanes like Newsletter
        var sub = new Shared.Models.Subscription
        {
            SubscriberId = created.Id,
            Email = created.Email,
            Name = created.Name
        };

        await _subscriberQueue.PublishSubscriberAsync(sub, ct);

        return created;
    }

    public async Task<bool> RemoveSubscriberAsync(int id, CancellationToken ct = default)
    {
        if (_repo is SubscriberRepository concrete)
            return await concrete.DeleteSubscriberAsync(id, ct);

        return false;
    }

    public async Task<List<SubscriberDto>> GetAllAsync(CancellationToken ct = default)
    {
        var subscribers = await _repo.GetAllSubScribersAsync();
        return subscribers.Select(s => new SubscriberDto(s.Id, s.Email, s.Name)).ToList();
    }
        
}