# Cloudflare Turnstile setup

Development automatically uses Cloudflare's public test keys. They always
validate successfully and must not be used in production.

Create a Managed Turnstile widget in Cloudflare and allow the production C5GO
hostnames. Store the generated values outside the repository:

```powershell
dotnet user-secrets set "Turnstile:SiteKey" "YOUR_SITE_KEY" --id "C5GO-Website-Diplomski"
$TurnstileSecret = Read-Host "Turnstile secret key"
dotnet user-secrets set "Turnstile:SecretKey" $TurnstileSecret --id "C5GO-Website-Diplomski"
Remove-Variable TurnstileSecret
```

For the VPS, configure environment variables instead:

```text
Turnstile__SiteKey=YOUR_SITE_KEY
Turnstile__SecretKey=YOUR_SECRET_KEY
```

Production startup fails when either value is missing. The secret key must
never be committed to Git or exposed to browser code.
