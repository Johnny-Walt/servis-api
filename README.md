# ServisAPI 🔌

REST API za ServisAplikaciju. Pokreće se na Railway.app s MySQL bazom.

---

## 📁 Struktura

```
ServisAPI/
├── Models/         → Popravak model + DTOs
├── Data/           → DbContext (MySQL)
├── Endpoints/      → API rute
├── Services/       → TrackingKodService
├── Program.cs      → Glavni entry point
├── Dockerfile      → Za Railway deployment
└── appsettings.json
```

---

## 🚀 Deployment na Railway (korak po korak)

### 1. Kreiraj GitHub repozitorij

```bash
# U folderu ServisAPI:
git init
git add .
git commit -m "Initial API commit"
git branch -M main
git remote add origin https://github.com/TVOJE_IME/servis-api.git
git push -u origin main
```

### 2. Kreiraj Railway projekt

1. Idi na **railway.app** i prijavi se GitHub accountom
2. Klikni **"New Project"**
3. Odaberi **"Deploy from GitHub repo"**
4. Odaberi tvoj `servis-api` repozitorij
5. Railway automatski detektira Dockerfile i gradi projekt

### 3. Dodaj MySQL bazu

1. U Railway projektu klikni **"+ New"**
2. Odaberi **"Database" → "MySQL"**
3. Railway automatski kreira bazu i varijable

### 4. Poveži API s bazom

1. Klikni na tvoj API servis u Railwayu
2. Idi na **"Variables"** tab
3. Dodaj varijablu:
   ```
   DATABASE_URL = mysql://user:password@host:port/dbname
   ```
   (Railway automatski prikaže connection string kad klikneš na MySQL servis → "Connect")

### 5. Deploy!

Railway automatski deployira svaki push na `main` granu.

Nakon deploymenta dobit ćeš URL poput:
```
https://servis-api-production-xxxx.railway.app
```

---

## 🔗 API Endpoints

| Metoda | URL | Opis |
|--------|-----|------|
| GET | `/api/popravci/aktivni` | Aktivni popravci |
| GET | `/api/popravci/rijeseni` | Riješeni popravci |
| GET | `/api/popravci/arhiva` | Arhiva |
| GET | `/api/popravci/pretraga?q=tekst` | Pretraga |
| GET | `/api/popravci/{id}` | Jedan popravak |
| **GET** | **`/api/popravci/track/{kod}`** | **Javni tracking (za web)** |
| POST | `/api/popravci` | Novi popravak |
| PATCH | `/api/popravci/{id}` | Ažuriraj |
| DELETE | `/api/popravci/{id}` | Arhiviraj (soft delete) |

Swagger dokumentacija dostupna na: `https://tvoj-url.railway.app/swagger`

---

## ⚙️ Konfiguracija Desktop aplikacije

Nakon deploymenta, u Desktop aplikaciji:
1. Otvori **Postavke**
2. Unesi API URL (npr. `https://servis-api-production-xxxx.railway.app`)
3. Spremi → aplikacija se automatski spaja

---

## 🔒 Sigurnost (napomena)

Trenutno API nema autentifikaciju (da bude jednostavno za početak).

Za produkciju preporučujem dodati API Key autentifikaciju:
- Desktop app šalje `X-API-Key` header
- Web tracking stranica koristi javni `/track/` endpoint bez keya

Mogu dodati ovo u sljedećoj fazi ako želiš.

---

## 📊 Tracking endpoint (za web stranicu)

```
GET /api/popravci/track/SR-2025-00042
```

Vraća samo JAVNE podatke (bez cijene, napomene):
```json
{
  "trackingKod": "SR-2025-00042",
  "statusNaziv": "U toku",
  "statusBoja": "#673AB7",
  "statusBroj": 4,
  "modelMobitela": "Samsung Galaxy S23",
  "punoIme": "Ivan Horvat",
  "datumZaprimanja": "2025-02-15T10:30:00Z",
  "datumZavrsetka": null
}
```
