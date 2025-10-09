# Multi-stage Dockerfile for building and running the .NET 9 Blazor app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore first for layer caching
COPY ["cse325-Team6-Project.csproj", "./"]
RUN dotnet restore "./cse325-Team6-Project.csproj"

# Copy the rest and publish
COPY . .
RUN dotnet publish "cse325-Team6-Project.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Recommended: Program.cs already reads the PORT env var Render provides
# and binds to 0.0.0.0. No explicit ASPNETCORE_URLS required here.

ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose a default port for local readability (Render will provide PORT at runtime)
EXPOSE 5000

ENTRYPOINT ["dotnet", "cse325-Team6-Project.dll"]