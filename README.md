# C5GO

[![C5GO CI](https://github.com/Dubsteer/C5GO/actions/workflows/main_c5g0.yml/badge.svg)](https://github.com/Dubsteer/C5GO/actions/workflows/main_c5g0.yml)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![MySQL 8.4](https://img.shields.io/badge/MySQL-8.4-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![Docker](https://img.shields.io/badge/Docker-ready-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Full-stack ASP.NET Core 10 platform for CS2 tournaments, teams, matches and
community features, built with MySQL, Docker and Cloudflare Turnstile.

## Features

- Account registration, email verification, login and password recovery
- Player profiles with Steam ID validation and completed-match history
- Team creation, captain-managed membership requests and team rosters
- Solo and team tournaments with registration, bracket generation and progression
- Public match pages plus optional live CS2 match data from PandaScore
- News posts, comments and in-app notifications
- Community discussions, voting, replies, reporting and moderation
- Owner, Admin, Moderator and Member roles with role-aware administration
- Cloudflare Turnstile, rate limiting, secure cookies and persistent data-protection keys
- Reproducible Docker Compose setup with MySQL health checks and sanitized demo data

## Architecture

```text
Browser
   |
   v
Website (ASP.NET Core Razor Pages)
   |
   v
LogicLayer (managers, policies and domain models)
   |
   v
DataAccessLayer (repository implementations)
   |
   v
MySQL 8.4
```

The web project owns presentation, authentication and dependency injection.
Business rules stay in `LogicLayer`, while database access is isolated behind
repository interfaces and MySQL implementations in `DataAccessLayer`.

## Tech stack

| Area | Technology |
| --- | --- |
| Backend | C# 14, ASP.NET Core 10, Razor Pages |
| Database | MySQL 8.4, MySql.Data |
| Frontend | Razor, HTML, CSS, JavaScript, Bootstrap |
| Integrations | PandaScore API, Cloudflare Turnstile, SMTP |
| Infrastructure | Docker, Docker Compose, Cloudflare Tunnel |
| Quality | MSTest, GitHub Actions, `dotnet format` |

## Testing and CI

The automated suite contains **207 tests** covering domain logic, repositories,
authentication-related services, tournament brackets, PandaScore integration and
web integration behavior.

The [C5GO CI workflow](https://github.com/Dubsteer/C5GO/actions/workflows/main_c5g0.yml)
runs on every push and pull request to `main`. It restores dependencies, verifies
formatting and analyzers, builds with warnings treated as errors, runs all tests,
publishes the website, builds the Docker images and smoke-tests the complete
website/MySQL environment. The referenced
[207-test CI run](https://github.com/Dubsteer/C5GO/actions/runs/32973182297)
completed successfully.

Run the test suite locally:

```powershell
dotnet test Unit_Tests/Unit_Tests.csproj --configuration Release
```

## Run with Docker

### Prerequisites

- Docker Desktop configured to use Linux containers
- Available local port `5063`

Create the local configuration from the supplied template:

```powershell
Copy-Item .env.example .env
```

Replace the example database and email values in `.env`, then start the website
and MySQL:

```powershell
docker compose up --build --detach --wait
```

Open [http://localhost:5063](http://localhost:5063). The health endpoint is
available at [http://localhost:5063/health](http://localhost:5063/health).

Useful commands:

```powershell
docker compose ps
docker compose logs --follow website
docker compose down
```

`docker compose down` preserves the database, uploads and data-protection keys in
named volumes. The database schema is imported automatically when a new empty
database volume is created.

To start the sanitized evaluation dataset, add the supervisor override:

```powershell
docker compose -f compose.yaml -f compose.professor.yaml up --build --detach --wait
```

For the complete evaluation workflow, see the
[Docker review guide](docs/professor-docker-guide.md).

## Run without Docker

Local Visual Studio development uses .NET User Secrets. Keep connection strings,
SMTP credentials, Turnstile keys and the PandaScore token outside committed
configuration files. See [security and deployment](docs/security-and-deployment.md)
and [Turnstile setup](docs/turnstile-setup.md) for the required settings.

## Documentation

- [Docker review guide](docs/professor-docker-guide.md)
- [Security and deployment](docs/security-and-deployment.md)
- [Cloudflare Turnstile setup](docs/turnstile-setup.md)
- [Database hardening](docs/database-hardening.md)
- [Community foundation](docs/community-foundation.md)
- [Web feature parity](docs/web-feature-parity.md)

## License

This project is available under the [MIT License](LICENSE).
