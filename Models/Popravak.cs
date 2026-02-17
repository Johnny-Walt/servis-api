using System.ComponentModel.DataAnnotations;

namespace ServisAPI.Models
{
    public enum StatusPopravka
    {
        Zaprimljeno = 0,
        Dijagnostika = 1,
        NaCekanju = 2,
        CekaSeDio = 3,
        UTokuPopravak = 4,
        Testiranje = 5,
        Spremno = 6,
        Preuzeto = 7,
        Otkazano = 8
    }

    public class Popravak
    {
        [Key]
        public int Id { get; set; }

        public string TrackingKod { get; set; } = string.Empty;

        [Required]
        public string Ime { get; set; } = string.Empty;

        [Required]
        public string Prezime { get; set; } = string.Empty;

        public string BrojMobitela { get; set; } = string.Empty;

        [Required]
        public string ModelMobitela { get; set; } = string.Empty;

        [Required]
        public string OpisKvara { get; set; } = string.Empty;

        public string Napomena { get; set; } = string.Empty;

        public decimal DogovorenaciJena { get; set; }

        public DateTime DatumZaprimanja { get; set; } = DateTime.UtcNow;

        public DateTime? DatumZavrsetka { get; set; }

        public StatusPopravka Status { get; set; } = StatusPopravka.Zaprimljeno;

        public bool Arhiviran { get; set; } = false;

        // Computed (not stored in DB)
        public string PunoIme => $"{Ime} {Prezime}";

        public string StatusNaziv => Status switch
        {
            StatusPopravka.Zaprimljeno   => "Zaprimljeno",
            StatusPopravka.Dijagnostika  => "Dijagnostika",
            StatusPopravka.NaCekanju     => "Na čekanju",
            StatusPopravka.CekaSeDio     => "Čeka se dio",
            StatusPopravka.UTokuPopravak => "U toku",
            StatusPopravka.Testiranje    => "Testiranje",
            StatusPopravka.Spremno       => "Spremno",
            StatusPopravka.Preuzeto      => "Preuzeto",
            StatusPopravka.Otkazano      => "Otkazano",
            _ => "Nepoznato"
        };

        public string StatusBoja => Status switch
        {
            StatusPopravka.Zaprimljeno   => "#787878",
            StatusPopravka.Dijagnostika  => "#2196F3",
            StatusPopravka.NaCekanju     => "#FF9800",
            StatusPopravka.CekaSeDio     => "#FF5722",
            StatusPopravka.UTokuPopravak => "#673AB7",
            StatusPopravka.Testiranje    => "#00BCD4",
            StatusPopravka.Spremno       => "#4CAF50",
            StatusPopravka.Preuzeto      => "#607D8B",
            StatusPopravka.Otkazano      => "#F44336",
            _ => "#787878"
        };
    }

    // DTOs
    public class NoviPopravakDto
    {
        [Required] public string Ime { get; set; } = string.Empty;
        [Required] public string Prezime { get; set; } = string.Empty;
        public string BrojMobitela { get; set; } = string.Empty;
        [Required] public string ModelMobitela { get; set; } = string.Empty;
        [Required] public string OpisKvara { get; set; } = string.Empty;
        public string Napomena { get; set; } = string.Empty;
        public decimal DogovorenaciJena { get; set; }
        public DateTime? DatumZaprimanja { get; set; }
    }

    public class AzurirajPopravakDto
    {
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? BrojMobitela { get; set; }
        public string? ModelMobitela { get; set; }
        public string? OpisKvara { get; set; }
        public string? Napomena { get; set; }
        public decimal? DogovorenaciJena { get; set; }
        public StatusPopravka? Status { get; set; }
        public bool? Arhiviran { get; set; }
    }

    public class TrackingOdgovorDto
    {
        public string TrackingKod { get; set; } = string.Empty;
        public string StatusNaziv { get; set; } = string.Empty;
        public string StatusBoja { get; set; } = string.Empty;
        public int StatusBroj { get; set; }
        public string ModelMobitela { get; set; } = string.Empty;
        public string PunoIme { get; set; } = string.Empty;
        public DateTime DatumZaprimanja { get; set; }
        public DateTime? DatumZavrsetka { get; set; }
        // Ne vraćamo cijenu i ostale osjetljive podatke javno
    }
}
