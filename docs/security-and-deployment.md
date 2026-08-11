# C5GO security and deployment

## Configuration

Secrets must be supplied through .NET User Secrets during local development and through environment variables on the VPS. They must never be entered into an appsettings file.

Required production variables:

```text
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://127.0.0.1:5063
ConnectionStrings__DefaultConnection=<MySQL connection string>
EmailSettings__SenderEmail=<sender address>
EmailSettings__Username=<SMTP username>
EmailSettings__Password=<Google App Password>
PandaScore__ApiKey=<PandaScore token>
DataProtection__KeysPath=/var/lib/c5go/data-protection-keys
```

The data-protection directory must be persistent across application restarts and readable and writable only by the operating-system account that runs C5GO. Losing these keys invalidates authentication cookies and active password-reset links.

The current appsettings file contains no secrets. Older Git commits contained the former database connection string and former Gmail credentials. The Gmail App Password has been replaced. The MySQL user's password must also be changed before public deployment.

## Cloudflare Tunnel and VPS

The ASP.NET application should listen only on `127.0.0.1`. `cloudflared` maps the public hostname to `http://127.0.0.1:5063`, so no public inbound web port is required. MySQL should also listen only locally unless a separately protected private network is intentionally configured.

Only the local proxy is trusted for `X-Forwarded-For` and `X-Forwarded-Proto`. This allows HTTPS links, secure cookies and per-client rate limiting to work correctly behind the tunnel without trusting headers sent directly by visitors.

Use `GET /health` for a basic process check. It intentionally reveals no configuration, database or user data.

Before the defense:

1. Run the app with the Production environment and verify the public HTTPS address.
2. Verify login, logout, registration, email verification and password reset through the public hostname.
3. Verify `/health` returns HTTP 200 through the tunnel.
4. Restrict the VPS firewall and confirm the app and MySQL ports are not publicly reachable.
5. Configure `cloudflared` and the C5GO service to start automatically after a reboot.
6. Back up the database and test restoring it.

## Database review

Run `docs/database-security-audit.sql` in MySQL Workbench. It is read-only and reports storage engines, indexes, duplicate identity values, invalid old Steam IDs, duplicate memberships, orphaned records, invalid status or score values and foreign-key rules.

The reviewed migration order and current local audit summary are in `docs/database-hardening.md`. Phase 1 can be applied only after a verified backup and a clean phase 1 audit. Phase 2 must wait until duplicate identity values and invalid legacy SteamIDs have been reviewed manually.
