using NewsletterService.Data;
using NewsletterService.Models;
using NewsletterService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace NewsletterService.Repositories;

public class SubscriberRepository : ISubscriberRepository
{
    private readonly NewsletterDbContext _context;
    public SubscriberRepository(NewsletterDbContext context) => _context = context;

    public async Task<Subscriber> AddSubscriberAsync(Subscriber subscriber)
    {
        _context.Subscribers.Add(subscriber);
        await _context.SaveChangesAsync();
        return subscriber;
    }

    public async Task<List<Subscriber>> GetAllSubScribersAsync() => await _context.Subscribers.ToListAsync();
}