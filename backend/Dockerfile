# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage Docker layer cache
COPY TransparenciaPE.sln ./
COPY src/TransparenciaPE.API/TransparenciaPE.API.csproj src/TransparenciaPE.API/
COPY src/TransparenciaPE.Application/TransparenciaPE.Application.csproj src/TransparenciaPE.Application/
COPY src/TransparenciaPE.Domain/TransparenciaPE.Domain.csproj src/TransparenciaPE.Domain/
COPY src/TransparenciaPE.Infrastructure/TransparenciaPE.Infrastructure.csproj src/TransparenciaPE.Infrastructure/

RUN dotnet restore src/TransparenciaPE.API/TransparenciaPE.API.csproj

# Copy everything else and publish
COPY . .
RUN dotnet publish src/TransparenciaPE.API/TransparenciaPE.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TransparenciaPE.API.dll"]
