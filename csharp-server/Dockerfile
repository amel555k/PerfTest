# version must match with project (in this case 9.0)
ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build

# only .csproj files, ther restore: this layered is caching while are packages are unchagned
COPY src/EStore.Domain/*.csproj             src/EStore.Domain/
COPY src/EStore.Application/*.csproj        src/EStore.Application/
COPY src/EStore.Infrastructure/*.csproj     src/EStore.Infrastructure/
COPY src/EStore.Api/*.csproj                src/EStore.Api/
RUN dotnet restore src/EStore.Api

# after that source code
COPY src/ src/
RUN dotnet publish src/EStore.Api -c Release -o /app --no-restore -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app

# curl only for Docker healthcheck on /health
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8083
EXPOSE 8083
USER $APP_UID
ENTRYPOINT ["dotnet", "EStore.Api.dll"]
