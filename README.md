# Happy Head Lines - A Project of Microservices and Containers.

## Introduction
This project contains Microservices that each has their own role for a system based around Publishing and Reading Articles and their comments.
The project uses Entity Framework to do code-first databases through migrations and having specific requirements for each microservice.

## Specialities
DraftService contains Serilogging and tracing using zipkin.

ArticleService contains x-axis scaling and y-axes scaling as well as ArticleCache.

ProfanityService contains fault isolation, so if it fails, the CommentService won't.

CommentService contains fault isolation regarding ProfanityService and CommentCache.

PublisherService contains ArticleQueue.

NewsletterService contains subscriber to ArticleQueue, 

Shared contains the RedisHelper, the LoggingHelper(Serilog and tracing), ArticleQueue, DTOs, RabbitHelper, PublishArticle Model. Accessible to all Services.

Testing contains some unit testing.

## Technologies used
VS Code IDE, Microsoft SQL, Docker, GitHub, ASP.NET Core, EF Core, Prometheus+Grafana, Zipkin, Serilog.

## Run
Build the docker environment:
```
docker-compose up -d
```
Go to each service and apply migration to get the Database.
```
dotnet ef database update
```
Running the above while keeping it docker-friendly. ServiceDB depends on which service you are inside. User is your DB user, Password is your db password.
```
dotnet ef database update --connection "Server=localhost,1435;Database={ServiceDB};User Id={User};Password={Password};TrustServerCertificate=True;"
```





