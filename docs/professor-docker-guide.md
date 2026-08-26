# C5GO — uputstvo za pregled Docker verzije

Arhiva sadrži izvorni kod ASP.NET Core 10 web aplikacije, MySQL 8.4 bazu,
automatizovane testove i Docker konfiguraciju. Ne sadrži Git istoriju niti
privatne Gmail, Cloudflare ili PandaScore podatke.

## Pokretanje

Potrebni su Docker Desktop sa uključenim Linux containers režimom i slobodan
lokalni port `5063`. Git, Visual Studio i zasebna instalacija MySQL-a nisu
potrebni.

Raspakovati arhivu i otvoriti PowerShell u direktorijumu `C5GO`.

Napraviti lokalnu konfiguraciju iz bezbjednog demonstracionog predloška:

```powershell
Copy-Item .env.professor.example .env
```

Pokrenuti web aplikaciju, bazu i demonstracione podatke:

```powershell
docker compose -f compose.yaml -f compose.professor.yaml up --build --detach --wait
```

Provjeriti stanje servisa:

```powershell
docker compose -f compose.yaml -f compose.professor.yaml ps
Invoke-RestMethod http://localhost:5063/health
```

Očekivani rezultat health provjere je `healthy`. Aplikacija je dostupna na
[http://localhost:5063](http://localhost:5063).

## Demonstracioni nalog

Demonstracioni administrator je unaprijed potvrđen i namijenjen je isključivo
lokalnom pregledu:

```text
Username: profesor
Password: C5G0-Demo-2026!
```

Baza sadrži izmišljene korisnike sa validnim formatom SteamID64, dvije kompletne
ekipe, završen individualni turnir sa bracketom, otvorene turnire, vijesti,
komentare, diskusije, glasove i obavještenja. Ne sadrži podatke stvarnih
korisnika.

## Zaustavljanje i ponovno pokretanje

Zaustavljanje aplikacije:

```powershell
docker compose -f compose.yaml -f compose.professor.yaml down
```

Ova komanda čuva bazu, postavljene slike i aplikacione ključeve u Docker
volumenima. Ponovno pokretanje:

```powershell
docker compose -f compose.yaml -f compose.professor.yaml up --detach --wait
```

Ako je izvorni kod promijenjen, potrebno je ponovo izgraditi image:

```powershell
docker compose -f compose.yaml -f compose.professor.yaml up --build --detach --wait
```

Komanda ispod trajno briše lokalnu demonstracionu bazu, slike i ključeve. Koristi
se samo kada je potrebna potpuno nova instalacija:

```powershell
docker compose -f compose.yaml -f compose.professor.yaml down --volumes
```

## Spoljni servisi

Cloudflare Turnstile koristi zvanične testne ključeve i radi lokalno. Ako
PandaScore token nije unesen, aplikacija prikazuje ugrađene demonstracione
profesionalne mečeve.

Slanje stvarnih verifikacionih i reset mejlova je namjerno isključeno jer arhiva
ne sadrži privatni Gmail App Password. Demonstracioni administrator je već
potvrđen, pa su prijava i pregled zaštićenih funkcija odmah dostupni.

U javnoj verziji aplikacije verifikacioni mejl vodi na `auth.c5g0.com`. Taj link
je dostupan kada vlasnik projekta pokrene Docker aplikaciju i Cloudflare Tunnel.
Sama logika verifikacije može se lokalno testirati sa sopstvenim SMTP nalogom i
vrijednošću `APP_AUTH_URL=http://localhost:5063`.

## Arhitektura Docker okruženja

Docker Compose pokreće dva servisa:

- ASP.NET Core 10 web aplikaciju;
- MySQL 8.4 bazu.

Baza je dostupna samo web aplikaciji preko privatne Docker mreže. MySQL port nije
objavljen računaru domaćinu niti internetu. Web aplikacija je vezana za
`127.0.0.1:5063`, a podaci se čuvaju u imenovanim Docker volumenima.

## Dijagnostika

```powershell
docker compose -f compose.yaml -f compose.professor.yaml logs --follow website
docker compose -f compose.yaml -f compose.professor.yaml logs --follow database
```

Praćenje loga prekida se kombinacijom `Ctrl+C`; kontejneri nastavljaju da rade.
