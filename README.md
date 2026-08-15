# C5GO

C5GO is an ASP.NET Core 10 web platform for CS2 tournaments, teams, matches,
news and community discussions. MySQL is used for persistent application data.

## Run with Docker

Prerequisites:

- Docker Desktop configured to use Linux containers
- available local port `5063`

Create the local configuration from the provided template:

```powershell
Copy-Item .env.example .env
```

Open `.env` and replace the example database and email values. The committed
template contains Cloudflare Turnstile test keys intended only for local testing.
The real `.env` file is ignored by Git and must never be committed or sent with
real credentials.

Build and start the website and MySQL:

```powershell
docker compose up --build --detach --wait
```

The website is available at [http://localhost:5063](http://localhost:5063) and
the health endpoint at [http://localhost:5063/health](http://localhost:5063/health).

Useful commands:

```powershell
docker compose ps
docker compose logs --follow website
docker compose down
```

`docker compose down` stops the project but preserves the database, uploaded
images and data-protection keys in named Docker volumes. The schema is imported
automatically only when a new empty database volume is created.

The following command permanently deletes the Docker database, uploads and
keys. Use it only when a completely clean installation is intended:

```powershell
docker compose down --volumes
```

The website is published only on the local host interface. An existing
Cloudflare Tunnel that points to `http://localhost:5063` can therefore expose it
without publishing the MySQL port or opening the website port to the local
network.

## Run without Docker

Local Visual Studio development continues to use .NET User Secrets. Docker does
not change the existing Visual Studio workflow.
