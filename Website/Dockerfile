FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Website/Website.csproj", "Website/"]
COPY ["DataAccessLayer/DataLayer.csproj", "DataAccessLayer/"]
COPY ["LogicLayer/LogicLayer.csproj", "LogicLayer/"]
RUN dotnet restore "Website/Website.csproj"

COPY ["Website/", "Website/"]
COPY ["DataAccessLayer/", "DataAccessLayer/"]
COPY ["LogicLayer/", "LogicLayer/"]
RUN dotnet publish "Website/Website.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /var/lib/c5go/uploads /var/lib/c5go/keys \
    && chown -R app:app /var/lib/c5go

COPY --from=build --chown=app:app /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER app

HEALTHCHECK --interval=15s --timeout=5s --start-period=20s --retries=5 \
    CMD curl --fail --silent --show-error http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "Website.dll"]
