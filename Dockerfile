FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["JsonServiceService.sln", "./"]
COPY ["src/JsonServiceService.Api/JsonServiceService.Api.csproj", "src/JsonServiceService.Api/"]
COPY ["src/JsonServiceService.Core/JsonServiceService.Core.csproj", "src/JsonServiceService.Core/"]
COPY ["src/JsonServiceService.Models/JsonServiceService.Models.csproj", "src/JsonServiceService.Models/"]

# Restore dependencies
RUN dotnet restore "JsonServiceService.sln"

# Copy source code
COPY . .

# Build application
RUN dotnet build "JsonServiceService.sln" -c Release -o /app/build

# Publish application
FROM build AS publish
RUN dotnet publish "src/JsonServiceService.Api/JsonServiceService.Api.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80 443

ENTRYPOINT ["dotnet", "JsonServiceService.Api.dll"]