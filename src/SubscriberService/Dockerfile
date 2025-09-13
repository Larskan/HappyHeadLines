# SubscriberService/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

WORKDIR /src

# Copy project files
COPY ["src/SubscriberService/SubscriberService.csproj", "SubscriberService/"]
COPY ["src/Shared/Shared.csproj", "Shared/"]

# Restore dependencies
RUN dotnet restore "SubscriberService/SubscriberService.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/SubscriberService"
RUN dotnet build "SubscriberService.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "SubscriberService.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriberService.dll"]
