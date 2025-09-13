# ArticleService/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files relative to the root context
COPY src/ArticleService/ArticleService.csproj ArticleService/
COPY src/Shared/Shared.csproj Shared/

# Restore dependencies
WORKDIR /src/ArticleService
RUN dotnet restore "ArticleService.csproj"

# Copy ArticleService source files
COPY src/ArticleService/. .
COPY src/Shared/. ../Shared/ 

# Publish the project
RUN dotnet publish "ArticleService.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ArticleService.dll"]
