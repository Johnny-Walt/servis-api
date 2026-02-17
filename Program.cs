using Microsoft.EntityFrameworkCore;
using ServisAPI.Data;
using ServisAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// ── Baza podataka (MySQL) ────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("Connection string nije konfiguriran!");

builder.Services.AddDbContext<ServisDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ── CORS (dozvoli desktop app i web stranicu) ────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("ServisPolicy", policy =>
    {
        policy.AllowAnyOrigin()   // U produkciji zamijeniti s točnim domenom
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ── Swagger (API dokumentacija) ──────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "ServisAplikacija API",
        Version = "v1",
        Description = "API za upravljanje popravcima mobitela u servisu"
    });
});

var app = builder.Build();

// ── Automatska migracija baze pri pokretanju ─────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ServisDbContext>();
    db.Database.Migrate();
}

// ── Middleware ───────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServisAplikacija API v1");
    c.RoutePrefix = "swagger"; // API docs na /swagger
});

app.UseCors("ServisPolicy");

// ── Rute ─────────────────────────────────────────────────────────────────────
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapPopravciEndpoints();

app.Run();
