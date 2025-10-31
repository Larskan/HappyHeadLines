using FeatureHubSDK;
using Microsoft.Extensions.Logging;

namespace NewsletterService.FeatureFlags;

public class FeatureHubService
{
    private readonly ILogger<FeatureHubService> _logger;
    private readonly IFeatureHubRepository _repo;

    public FeatureHubService(ILogger<FeatureHubService> logger, IFeatureHubRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public bool IsSubscriberServiceOnline()
    {
        if (!_repo.Exists("SubscriberServiceActive"))
        {
            _logger.LogWarning("SubscriberServiceActive feature doesnt exist");
            return false;
        }
        return _repo.IsEnabled("SubscriberServiceActive");
    }
}