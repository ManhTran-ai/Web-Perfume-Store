# Use the official .NET 9.0 runtime image as base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["GuhaStore.Web/GuhaStore.Web.csproj", "GuhaStore.Web/"]
COPY ["GuhaStore.Application/GuhaStore.Application.csproj", "GuhaStore.Application/"]
COPY ["GuhaStore.Core/GuhaStore.Core.csproj", "GuhaStore.Core/"]
COPY ["GuhaStore.Infrastructure/GuhaStore.Infrastructure.csproj", "GuhaStore.Infrastructure/"]
RUN dotnet restore "GuhaStore.Web/GuhaStore.Web.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/GuhaStore.Web"
RUN dotnet build "GuhaStore.Web.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "GuhaStore.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: copy published app to base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser:appuser /app
USER appuser

ENTRYPOINT ["dotnet", "GuhaStore.Web.dll"]
