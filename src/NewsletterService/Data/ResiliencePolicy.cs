using Polly;
using Polly.Extensions.Http;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Net;

namespace NewsletterService.Data;

public static class ResiliencePolicy
{
    public static AsyncRetryPolicy AsyncRetryPolicy =>
        Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), //2sec, 4sec, 8sec, etc.
                onRetry: (exception, timespan, retryAttempt, context) =>
                {
                    Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to: {exception.Message}");
                });

    public static AsyncCircuitBreakerPolicy AsyncCircuitBreakerPolicy =>
        Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (ex, breakDelay) =>
                {
                    Console.WriteLine($"Circuit opened for {breakDelay.TotalSeconds}s due to: {ex.Message}");
                },
                onReset: () => Console.WriteLine("Circuit closed. Service recovered"),
                onHalfOpen: () => Console.WriteLine("Circuit half open: testing service health...")
            );
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(30));
    }
}