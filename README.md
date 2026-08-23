# MondakiComics — Backend API
## Tech Stack

- **ASP.NET Core Web API** (.NET 8)
- **PostgreSQL** με Entity Framework Core (Npgsql)
- **JWT Authentication** (Bearer tokens)
- **AutoMapper** για entity ↔ DTO mapping
- **Repository Pattern** + **Unit of Work**
- **Serilog** για structured logging
- **Cloudflare R2** (S3-compatible) για αποθήκευση εικόνων
- **BCrypt** για password hashing
- **Swagger / OpenAPI** (μόνο σε development)

## Αρχιτεκτονική
Controllers/ → HTTP endpoints, routing, request/response
Services/ → Business logic
Repositories/ → Data access (EF Core)
DTO/ → Data transfer objects (input/output shapes)
Data/ → Entity models + DbContext
Configuration/ → AutoMapper profiles
Exceptions/ → Custom exceptions + global error handling middleware

Κάθε feature (Artworks, Categories, Users, ContactMessages, News) ακολουθεί το ίδιο pattern:
`Controller → Service → Repository → DbContext`

## Features

- **Artworks** — CRUD, categories, πολλαπλές εικόνες ανά artwork, cover image selection, publish/draft state
- **Categories** — CRUD, auto-slug generation
- **Contact Messages** — δημόσια φόρμα υποβολής, admin inbox με read/unread tracking
- **News Posts** — ανακοινώσεις με προαιρετική εικόνα, publish/draft state
- **Auth** — JWT-based login, role-based authorization (Admin)
- **Image Upload** — μέσω Cloudflare R2, με soft-delete support

## Environment Variables

Το backend διαβάζει configuration από environment variables (βλ. `Program.cs`):

| Variable | Περιγραφή |
|---|---|
| `MONDAKI_DB_HOST` | PostgreSQL host |
| `MONDAKI_DB_PORT` | PostgreSQL port |
| `MONDAKI_DB_NAME` | Database name |
| `MONDAKI_DB_USER` | Database user |
| `MONDAKI_DB_PASS` | Database password |
| `MONDAKI_JWT_SECRET` | Secret key για JWT signing (32+ χαρακτήρες) |
| `MONDAKI_JWT_ISSUER` | Issuer/Audience για JWT (π.χ. το production URL) |
| `MONDAKI_R2_ACCESS_KEY` | Cloudflare R2 access key |
| `MONDAKI_R2_SECRET_KEY` | Cloudflare R2 secret key |
| `MONDAKI_R2_ENDPOINT` | Cloudflare R2 S3 endpoint |
| `MONDAKI_R2_BUCKET` | Όνομα bucket |
| `MONDAKI_R2_PUBLIC_URL` | Public URL του bucket (για serving εικόνων) |

## Τοπική εκτέλεση

```bash
# Restore & build
dotnet restore
dotnet build

# Εφαρμογή migrations
dotnet ef database update

# Εκτέλεση
dotnet run
```

Swagger UI διαθέσιμο στο `/swagger` (μόνο σε Development environment).

## Migrations

```bash
# Δημιουργία νέου migration
dotnet ef migrations add <MigrationName>

# Εφαρμογή σε τοπική βάση
dotnet ef database update

# Εφαρμογή σε production βάση
dotnet ef database update --connection "Host=...;Port=...;Database=...;Username=...;Password=..."
```

## Deployment

Hosted στο **Railway** (PostgreSQL + backend service στο ίδιο project). Auto-deploys από το `master` branch.

## Security Notes

- Όλα τα write/mutate endpoints (Create/Update/Delete) απαιτούν `[Authorize(Roles = "Admin")]`
- Read-only public endpoints (Gallery, News, Contact submission) παραμένουν ανοιχτά σκόπιμα
- CORS περιορισμένο σε συγκεκριμένα origins (production frontend + localhost για development)
- Soft-delete pattern σε όλα τα entities (κανένα hard delete)
