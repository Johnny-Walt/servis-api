using Microsoft.EntityFrameworkCore;
using ServisAPI.Models;

namespace ServisAPI.Data
{
    public class ServisDbContext : DbContext
    {
        public ServisDbContext(DbContextOptions<ServisDbContext> options) : base(options) { }

        public DbSet<Popravak> Popravci { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Popravak>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DogovorenaciJena).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TrackingKod).HasMaxLength(50);
                entity.Property(e => e.Ime).HasMaxLength(100);
                entity.Property(e => e.Prezime).HasMaxLength(100);
                entity.Property(e => e.BrojMobitela).HasMaxLength(50);
                entity.Property(e => e.ModelMobitela).HasMaxLength(200);
                entity.Property(e => e.OpisKvara).HasMaxLength(2000);
                entity.Property(e => e.Napomena).HasMaxLength(2000);

                // Ignore computed properties
                entity.Ignore(e => e.PunoIme);
                entity.Ignore(e => e.StatusNaziv);
                entity.Ignore(e => e.StatusBoja);
            });
        }
    }
}
