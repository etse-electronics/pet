FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5112

ENV ASPNETCORE_URLS=http://+:5112
ENV ASPNETCORE_ENVIRONMENT=Production

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["webapi.csproj", "./"]
RUN dotnet restore "webapi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "webapi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "webapi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "webapi.dll"]

# docker build --pull --no-cache --rm -f "./Dockerfile" --platform=linux/amd64,linux/arm64 -t ghcr.io/edwin-nel/workshop-web-api:latest "."
# docker push ghcr.io/edwin-nel/workshop-web-api:latest