# DraftService/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
WORKDIR /src

# Copy project files
COPY ["src/DraftService/DraftService.csproj", "DraftService/"]
COPY ["src/Shared/Shared.csproj", "Shared/"]

# Restore dependencies
RUN dotnet restore "DraftService/DraftService.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/DraftService"
RUN dotnet build "DraftService.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "DraftService.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DraftService.dll"]
