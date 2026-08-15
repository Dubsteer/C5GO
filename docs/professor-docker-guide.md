# C5GO — uputstvo za pregled Docker verzije

Ovo uputstvo omogućava pregled aplikacije bez slanja privatnih lozinki, Gmail
App Password vrijednosti ili PandaScore tokena.

## Najbrži pregled

Reprezentativna verzija aplikacije dostupna je na
[https://c5g0.com](https://c5g0.com) dok su računar vlasnika projekta, Docker
Desktop i Cloudflare Tunnel uključeni.

Javna verzija sadrži pripremljene korisnike, timove, vijesti, diskusije i
završen turnir. Lokalna Docker instalacija namjerno počinje sa praznom bazom i
ne sadrži privatne podatke iz prezentacione baze.

## Lokalno pokretanje

Potrebni su:

- Git;
- Docker Desktop sa uključenim Linux containers režimom;
- slobodan lokalni port `5063`.

Klonirati repozitorijum i otvoriti njegov direktorijum:

```powershell
git clone https://github.com/Dubsteer/C5GO.git
cd C5GO
```

Napraviti lokalnu konfiguraciju:

```powershell
Copy-Item .env.example .env
notepad .env
```

U `.env` je dovoljno postaviti dvije različite lokalne MySQL lozinke:

```text
MYSQL_PASSWORD=odabrana-lokalna-lozinka
MYSQL_ROOT_PASSWORD=druga-odabrana-lokalna-lozinka
```

Cloudflare Turnstile vrijednosti iz predloška su javni testni ključevi i mogu
ostati nepromijenjene pri lokalnom pregledu. `PANDA_SCORE_API_KEY` može ostati
prazan; aplikacija tada koristi ugrađene demonstracione podatke o mečevima.

Vrijednosti `EMAIL_*` iz predloška omogućavaju pokretanje aplikacije, ali ne i
stvarno slanje mejla. Za provjeru registracije i promjene lozinke potreban je
vlastiti SMTP nalog. Privatni SMTP podaci vlasnika projekta nisu dio
repozitorijuma.

Pokrenuti MySQL i web aplikaciju:

```powershell
docker compose up --build --detach --wait
```

Provjeriti stanje kontejnera i health endpoint:

```powershell
docker compose ps
Invoke-RestMethod http://localhost:5063/health
```

Očekivani rezultat health provjere je `healthy`. Aplikacija je dostupna na
[http://localhost:5063](http://localhost:5063).

## Zaustavljanje i ponovno pokretanje

Zaustavljanje aplikacije:

```powershell
docker compose down
```

Ova komanda čuva bazu, postavljene slike i ključeve aplikacije u Docker
volumenima. Ponovno pokretanje:

```powershell
docker compose up --detach --wait
```

Ako je izvorni kod promijenjen, ponovo izgraditi image:

```powershell
docker compose up --build --detach --wait
```

## Korisne dijagnostičke komande

```powershell
docker compose logs --follow website
docker compose logs --follow database
docker compose ps
```

Praćenje loga se prekida kombinacijom `Ctrl+C`; kontejneri nastavljaju da rade.

## Bezbjednosne napomene

- `.env` se ne šalje i ne postavlja na GitHub;
- repozitorijum sadrži samo `.env.example` sa primjerima i testnim ključevima;
- MySQL port nije izložen računaru domaćinu niti internetu;
- web port je vezan samo za `127.0.0.1`;
- javni pristup se ostvaruje kroz Cloudflare Tunnel;
- stvarni Gmail App Password i PandaScore token ostaju samo kod vlasnika
  projekta.

Docker Compose pokreće dva osnovna servisa: ASP.NET Core 10 web aplikaciju i
MySQL 8.4 bazu. Baza je dostupna samo web aplikaciji preko privatne Docker
mreže. Podaci se čuvaju u imenovanim volumenima i ostaju sačuvani nakon običnog
zaustavljanja kontejnera.
