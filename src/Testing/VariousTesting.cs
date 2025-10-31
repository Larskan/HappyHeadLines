using Xunit;
using Moq;
using CommentService.Interfaces;
using CommentService.Models;
using CommentService.Services;
using System.Net.Http;
using System.Threading.Tasks;
using Polly.CircuitBreaker;
using System.Net;
using Shared;
using ArticleService.Helpers;
using Moq.Protected;
using ArticleService.Interfaces;
using Serilog;
using Microsoft.Extensions.Logging;
using ArticleService.Models;
using CommentService.Data;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore;
using DraftService.Data;
using PublisherService.Repositories;
using Shared.Models;
using System.Threading;
using Microsoft.AspNetCore.Identity.UI.Services;
using NewsletterService.Services;
using FeatureHubSDK;
using NewsletterService.FeatureFlags;



namespace Testing;

public class VariousTesting
{
    [Fact]
    public async Task AddComment_WhenProfanityFails_ShouldStillSaveComment()
    {
        // Arrange
        var mockRepo = new Mock<ICommentRepository>();
        mockRepo.Setup(r => r.CreateCommentAsync(It.IsAny<Comment>())).ReturnsAsync((Comment c) => c);

        // simulate HttpClient for ProfanityService that always fails
        var mockFactory = new Mock<IHttpClientFactory>();
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(handlerMock.Object);
        httpClient.BaseAddress = new Uri("http://profanity-service:8080");
        mockFactory.Setup(f => f.CreateClient("Profanity")).Returns(httpClient);

        // handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ThrowsAsync(new HttpRequestException("ProfanityService down"));

        var service = new CommentService.Services.CommentService(mockRepo.Object, mockFactory.Object);
        var dto = new CommentDto(1, 1, "Lars Test", "Hello", DateTime.UtcNow);

        // Act
        var result = await service.CreateCommentAsync(1, dto);

        // Assert
        Assert.Equal(dto.Body, result.Body); // Body not filtered
        Assert.False(result.Body != dto.Body); //not filtered
        mockRepo.Verify(r => r.CreateCommentAsync(It.IsAny<Comment>()), Times.Once);
    }

    [Fact]
    // Ensures cache is checked before repo, repo is never called.
    public async Task GetArticle_ShouldReturnFromCacheBeforeRepo()
    {
        // Arrange
        var mockRedis = new Mock<IRedisHelper>();
        var mockRepo = new Mock<IArticleRepository>();
        var mockLogger = new Mock<ILogger<ArticleCache>>();
        var cachedArticle = new Article { Id = 1, Title = "Cached", Body = "From cache" };
        mockRedis.Setup(r => r.GetAsync<Article>("article:1")).ReturnsAsync(cachedArticle);
        var cache = new ArticleCache(mockRedis.Object, mockRepo.Object, mockLogger.Object);

        // Act
        var result = await cache.GetArticlesFromCacheFirstAsync(1, "Global");

        // Assert
        Assert.Equal(cachedArticle.Title, result!.Title);
        mockRepo.Verify(r => r.GetArticlesSinceAsync(It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task GetCommentsAsync_ShouldReturnFromCache_IfAvailable()
    {
        // Arrange
        var cachedComments = new List<CommentDto>
        {
            new CommentDto(1, 1, "Lars Test", "Hello", DateTime.UtcNow)
        };
        var mockRedis = new Mock<IRedisHelper>();
        var mockRepo = new Mock<ICommentRepository>();

        mockRedis.Setup(r => r.GetAsync<List<CommentDto>>("comments:1")).ReturnsAsync(cachedComments);

        var cache = new CommentCache(mockRedis.Object, mockRepo.Object);

        // Act
        var result = await cache.GetCommentsAsync(1);

        // Assert
        Assert.Single(result);
        mockRepo.Verify(r => r.GetByArticleIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task SubscriptionEventProcessor_ShouldSendWelcomeMail_WhenNewSubscriberReceived()
    {
        // Arrange
        var mockQueue = new Mock<ISubscriberQueueSubscriber>();
        var mockEmailSender = new Mock<IEmailSender>();
        var mockFeature = new Mock<FeatureHubService>();
        var logger = new Mock<ILogger<SubscriptionEventProcessor>>();

        var fakeSub = new Subscription
        {
            SubscriberId = 1,
            Email = "lars@example.com",
            Name = "Lars Hein"
        };

        // Mock SubscribeSubscriberAsync to invoke handler
        mockQueue
            .Setup(q => q.SubscribeSubscriberAsync(It.IsAny<Func<Subscription, CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Subscription, CancellationToken, Task>, CancellationToken>(async (handler, ct) =>
            {
                // Simulate receiving a new subscriber
                await handler(fakeSub, ct);
            });

        var processor = new SubscriptionEventProcessor(mockQueue.Object, logger.Object, mockEmailSender.Object, mockFeature.Object);

        // Act
        using var cts = new CancellationTokenSource(1000); // Short lived
        await processor.StartAsync(cts.Token);

        // Assert
        mockEmailSender.Verify(
            s => s.SendEmailAsync(
                "lars@example.com",
                It.Is<string>(subj => subj.Contains("Welcome")),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public void IsSubscriberServiceOnline_ReturnsTrue_WhenFeatureEnabled()
    {
        // Arrange
        var mockRepo = new Mock<IFeatureHubRepository>();
        mockRepo.Setup(r => r.Exists("SubscriberServiceActive")).Returns(true);
        mockRepo.Setup(r => r.IsEnabled("SubscriberServiceActive")).Returns(true);

        var mockLogger = new Mock<ILogger<FeatureHubService>>();
        var service = new FeatureHubService(mockLogger.Object, mockRepo.Object);

        // act
        var result = service.IsSubscriberServiceOnline();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSubscriberServiceOnline_ReturnsFalse_WhenFeatureDisabled()
    {
        // Arrange
        var mockRepo = new Mock<IFeatureHubRepository>();
        mockRepo.Setup(r => r.Exists("SubscriberServiceActive")).Returns(true);
        mockRepo.Setup(r => r.IsEnabled("SubscriberServiceActive")).Returns(false);

        var mockLogger = new Mock<ILogger<FeatureHubService>>();
        var service = new FeatureHubService(mockLogger.Object, mockRepo.Object);

        // act
        var result = service.IsSubscriberServiceOnline();

        // Assert
        Assert.False(result);
    }


}

