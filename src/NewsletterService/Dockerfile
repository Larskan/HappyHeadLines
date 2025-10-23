# NewsletterService/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY src/NewsletterService/NewsletterService.csproj NewsletterService/
COPY src/Shared/Shared.csproj Shared/

# Restore dependencies
WORKDIR /src/NewsletterService
RUN dotnet restore "NewsletterService.csproj"

# Copy everything else
COPY src/NewsletterService/. .
COPY src/Shared/. ../Shared/

# Build
# WORKDIR "/src/NewsletterService"
# RUN dotnet build "NewsletterService.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "NewsletterService.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NewsletterService.dll"]
