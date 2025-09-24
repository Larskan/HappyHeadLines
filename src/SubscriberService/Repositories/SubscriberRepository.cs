using SubscriberService.Data;
using SubscriberService.Models;
using SubscriberService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SubscriberService.Repositories;

public class SubscriberRepository : ISubscriberRepository
{
    private readonly SubscriberDbContext _context;
    public SubscriberRepository(SubscriberDbContext context) => _context = context;

    public async Task<Subscriber> AddSubscriberAsync(Subscriber subscriber)
    {
        _context.Subscribers.Add(subscriber);
        await _context.SaveChangesAsync();
        return subscriber;
    }

    public async Task<List<Subscriber>> GetAllSubScribersAsync() => await _context.Subscribers.ToListAsync();
}