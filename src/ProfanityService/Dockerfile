# ProfanityService/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy project files
COPY ["ProfanityService/ProfanityService.csproj", "ProfanityService/"]
COPY ["Shared/Shared.csproj", "Shared/"]

# Restore dependencies
RUN dotnet restore "ProfanityService/ProfanityService.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/ProfanityService"
RUN dotnet build "ProfanityService.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "ProfanityService.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProfanityService.dll"]
