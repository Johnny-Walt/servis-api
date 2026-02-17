using Microsoft.EntityFrameworkCore;
using ServisAPI.Data;
using ServisAPI.Models;
using ServisAPI.Services;

namespace ServisAPI.Endpoints
{
    public static class PopravciEndpoints
    {
        public static void MapPopravciEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/popravci")
                           .WithTags("Popravci");

            // ── GET sve aktivne (nije arhivirano, nije završeno) ──────────────
            group.MapGet("/aktivni", async (ServisDbContext db) =>
            {
                var lista = await db.Popravci
                    .Where(p => !p.Arhiviran
                        && p.Status != StatusPopravka.Preuzeto
                        && p.Status != StatusPopravka.Otkazano)
                    .OrderByDescending(p => p.DatumZaprimanja)
                    .ToListAsync();
                return Results.Ok(lista);
            })
            .WithSummary("Dohvati sve aktivne popravke");

            // ── GET riješeni ──────────────────────────────────────────────────
            group.MapGet("/rijeseni", async (ServisDbContext db) =>
            {
                var lista = await db.Popravci
                    .Where(p => !p.Arhiviran &&
                        (p.Status == StatusPopravka.Preuzeto || p.Status == StatusPopravka.Otkazano))
                    .OrderByDescending(p => p.DatumZavrsetka)
                    .ToListAsync();
                return Results.Ok(lista);
            })
            .WithSummary("Dohvati riješene popravke");

            // ── GET arhiva ────────────────────────────────────────────────────
            group.MapGet("/arhiva", async (ServisDbContext db) =>
            {
                var lista = await db.Popravci
                    .Where(p => p.Arhiviran)
                    .OrderByDescending(p => p.DatumZaprimanja)
                    .ToListAsync();
                return Results.Ok(lista);
            })
            .WithSummary("Dohvati arhivirane popravke");

            // ── GET pretraga ──────────────────────────────────────────────────
            group.MapGet("/pretraga", async (string q, ServisDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(q))
                    return Results.BadRequest("Unesite pojam pretrage.");

                q = q.ToLower();
                var lista = await db.Popravci
                    .Where(p => p.Ime.ToLower().Contains(q) ||
                                p.Prezime.ToLower().Contains(q) ||
                                p.ModelMobitela.ToLower().Contains(q) ||
                                p.TrackingKod.ToLower().Contains(q) ||
                                p.BrojMobitela.Contains(q))
                    .OrderByDescending(p => p.DatumZaprimanja)
                    .ToListAsync();
                return Results.Ok(lista);
            })
            .WithSummary("Pretraži popravke");

            // ── GET po ID-u ───────────────────────────────────────────────────
            group.MapGet("/{id:int}", async (int id, ServisDbContext db) =>
            {
                var p = await db.Popravci.FindAsync(id);
                return p is null ? Results.NotFound() : Results.Ok(p);
            })
            .WithSummary("Dohvati popravak po ID-u");

            // ── GET tracking (JAVNI endpoint za web stranicu) ─────────────────
            group.MapGet("/track/{trackingKod}", async (string trackingKod, ServisDbContext db) =>
            {
                var p = await db.Popravci
                    .FirstOrDefaultAsync(x => x.TrackingKod == trackingKod);

                if (p is null) return Results.NotFound(new { poruka = "Popravak nije pronađen." });

                // Vraćamo samo javne podatke (bez cijene, napomene itd.)
                var odgovor = new TrackingOdgovorDto
                {
                    TrackingKod    = p.TrackingKod,
                    StatusNaziv    = p.StatusNaziv,
                    StatusBoja     = p.StatusBoja,
                    StatusBroj     = (int)p.Status,
                    ModelMobitela  = p.ModelMobitela,
                    PunoIme        = p.PunoIme,
                    DatumZaprimanja = p.DatumZaprimanja,
                    DatumZavrsetka  = p.DatumZavrsetka
                };
                return Results.Ok(odgovor);
            })
            .WithSummary("Javni tracking endpoint za web stranicu");

            // ── POST novi popravak ────────────────────────────────────────────
            group.MapPost("/", async (NoviPopravakDto dto, ServisDbContext db) =>
            {
                var popravak = new Popravak
                {
                    Ime              = dto.Ime.Trim(),
                    Prezime          = dto.Prezime.Trim(),
                    BrojMobitela     = dto.BrojMobitela.Trim(),
                    ModelMobitela    = dto.ModelMobitela.Trim(),
                    OpisKvara        = dto.OpisKvara.Trim(),
                    Napomena         = dto.Napomena.Trim(),
                    DogovorenaciJena = dto.DogovorenaciJena,
                    DatumZaprimanja  = dto.DatumZaprimanja?.ToUniversalTime() ?? DateTime.UtcNow,
                    Status           = StatusPopravka.Zaprimljeno,
                    Arhiviran        = false,
                    TrackingKod      = ""
                };

                db.Popravci.Add(popravak);
                await db.SaveChangesAsync();

                // Generiramo tracking kod sada kad imamo ID
                popravak.TrackingKod = TrackingKodService.Generiraj(popravak.Id);
                await db.SaveChangesAsync();

                return Results.Created($"/api/popravci/{popravak.Id}", popravak);
            })
            .WithSummary("Kreiraj novi popravak");

            // ── PATCH ažuriraj podatke / status ──────────────────────────────
            group.MapPatch("/{id:int}", async (int id, AzurirajPopravakDto dto, ServisDbContext db) =>
            {
                var p = await db.Popravci.FindAsync(id);
                if (p is null) return Results.NotFound();

                if (dto.Ime          != null) p.Ime              = dto.Ime.Trim();
                if (dto.Prezime      != null) p.Prezime          = dto.Prezime.Trim();
                if (dto.BrojMobitela != null) p.BrojMobitela     = dto.BrojMobitela.Trim();
                if (dto.ModelMobitela!= null) p.ModelMobitela    = dto.ModelMobitela.Trim();
                if (dto.OpisKvara    != null) p.OpisKvara        = dto.OpisKvara.Trim();
                if (dto.Napomena     != null) p.Napomena         = dto.Napomena.Trim();
                if (dto.DogovorenaciJena.HasValue) p.DogovorenaciJena = dto.DogovorenaciJena.Value;
                if (dto.Arhiviran.HasValue)   p.Arhiviran        = dto.Arhiviran.Value;

                if (dto.Status.HasValue)
                {
                    p.Status = dto.Status.Value;
                    if (dto.Status == StatusPopravka.Preuzeto || dto.Status == StatusPopravka.Otkazano)
                        p.DatumZavrsetka = DateTime.UtcNow;
                }

                await db.SaveChangesAsync();
                return Results.Ok(p);
            })
            .WithSummary("Ažuriraj popravak (parcijalni update)");

            // ── DELETE (soft – arhivira) ──────────────────────────────────────
            group.MapDelete("/{id:int}", async (int id, ServisDbContext db) =>
            {
                var p = await db.Popravci.FindAsync(id);
                if (p is null) return Results.NotFound();

                p.Arhiviran = true;
                await db.SaveChangesAsync();
                return Results.Ok(new { poruka = $"Popravak #{id} arhiviran." });
            })
            .WithSummary("Arhiviraj popravak (soft delete)");
        }
    }
}
