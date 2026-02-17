namespace ServisAPI.Services
{
    public class TrackingKodService
    {
        public static string Generiraj(int id)
        {
            return $"SR-{DateTime.UtcNow.Year}-{id:D5}";
        }
    }
}
