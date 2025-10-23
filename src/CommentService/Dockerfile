# CommentService/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY src/CommentService/CommentService.csproj CommentService/
COPY src/Shared/Shared.csproj Shared/

# Restore dependencies
WORKDIR /src/CommentService
RUN dotnet restore "CommentService.csproj"

# Copy everything else
COPY src/CommentService/. .
COPY src/Shared/. ../Shared/

# Build
# WORKDIR /src/CommentService
# RUN dotnet build "CommentService.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "CommentService.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CommentService.dll"]
