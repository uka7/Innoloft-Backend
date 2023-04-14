FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the solution file and all projects
COPY ["Innoloft.sln", "./"]
COPY ["Innoloft.Web/Innoloft.Web.csproj", "Innoloft.Web/"]
COPY ["Innoloft.Shared/Innoloft.Shared.csproj", "Innoloft.Shared/"]
COPY ["Innoloft.Cache.Redis/Innoloft.Cache.Redis.csproj", "Innoloft.Cache.Redis/"]
COPY ["Innoloft.Domain/Innoloft.Domain.csproj", "Innoloft.Domain/"]
COPY ["Innoloft.EntityFramework/Innoloft.EntityFramework.csproj", "Innoloft.EntityFramework/"]
COPY ["Innoloft.Tests/Innoloft.Tests.csproj", "Innoloft.Tests/"]

# Restore the packages for all projects
RUN dotnet restore

# Copy the source code of all projects
COPY . .

# Run database migrations
RUN dotnet new tool-manifest
RUN dotnet tool install dotnet-ef

# Build and publish the main web project
WORKDIR "/src/Innoloft.Web"
RUN dotnet build "Innoloft.Web.csproj" -c Release -o /app/build
RUN dotnet publish "Innoloft.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Innoloft.Web.dll"]
