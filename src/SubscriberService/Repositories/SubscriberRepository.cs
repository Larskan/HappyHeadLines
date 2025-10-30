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

    public async Task<bool> DeleteSubscriberAsync(int id, CancellationToken ct = default)
    {
        var s = await _context.Subscribers.FindAsync(new object[] { id }, ct);
        if (s == null) return false;
        _context.Subscribers.Remove(s);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<Subscriber>> GetAllSubScribersAsync() => await _context.Subscribers.ToListAsync();
}