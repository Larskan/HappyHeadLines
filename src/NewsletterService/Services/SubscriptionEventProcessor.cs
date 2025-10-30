using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace NewsletterService.Services;

public class SubscriptionEventProcessor : BackgroundService
{
    private readonly ISubscriberQueueSubscriber _subscriberQueue;
    private readonly ILogger<SubscriptionEventProcessor> _logger;
    private IEmailSender _emailSender; //email sender abstraction

    // If Subscriber is turned on, it will send welcome mails to new subscribers.
    public SubscriptionEventProcessor(ISubscriberQueueSubscriber subscriberQueue, ILogger<SubscriptionEventProcessor> logger, IEmailSender emailSender)
    {
        _subscriberQueue = subscriberQueue;
        _logger = logger;
        _emailSender = emailSender;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // if (!_featureHubContext["SubscriberServiceActive"].IsEnabled)
        // {
        //     _logger.LogInformation("SubscriberService is turned off. Skipping subscription processing...");
        //     return;
        // }
        try
        {
            //SubscribeAsync blocks until cancelletion
            await _subscriberQueue.SubscribeSubscriberAsync(async (subscription, token) =>
            {
                try
                {
                    _logger.LogInformation("Received new subscription {Email} (id: {SubscriberId})", subscription.Email, subscription.SubscriberId);

                    // Send welcome mail
                    var subject = "Welcome to HappyHeadlines!";
                    var body = $"<p>Hi {subscription.Name},</p><p>Thank you for subscribing to our newsletter!</p>";
                    await _emailSender.SendEmailAsync(subscription.Email!, subject, body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process subscription event for {Email}", subscription.Email);
                }
            }, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("SubscriptionEventProcessor is stopping due to cancellation.");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in SubscriptionEventProcessor");
        }
        
        
    }
}