using Microsoft.EntityFrameworkCore;
using NewsletterService.Data;
using NewsletterService.Models;
using NewsletterService.Interfaces;
using Shared;
using Serilog;
using System.Net.Http.Json;

namespace NewsletterService.Repositories;

public class NewsletterSubscriberRepository : INewsletterSubscriberRepository
{
    private readonly HttpClient _http;
    private readonly ILogger<NewsletterSubscriberRepository> _logger;

    public NewsletterSubscriberRepository(HttpClient http, ILogger<NewsletterSubscriberRepository> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<List<SubscriberDto>> GetAllSubscribersAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("/api/subscriber/subscribers", ct);
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<SubscriberDto>>(cancellationToken: ct);
        return list ?? new List<SubscriberDto>();
    }
}