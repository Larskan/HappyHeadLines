# PublisherService/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY src/PublisherService/PublisherService.csproj PublisherService/
COPY src/Shared/Shared.csproj /Shared/

# Restore dependencies
WORKDIR /src/PublisherService
RUN dotnet restore "PublisherService.csproj"

# Copy everything else
COPY src/PublisherService/. .
COPY src/Shared/. ../Shared/

# Build
# WORKDIR "/src/PublisherService"
# RUN dotnet build "PublisherService.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "PublisherService.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PublisherService.dll"]
