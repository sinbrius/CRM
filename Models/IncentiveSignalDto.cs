namespace CrmDb.Models
{
    public class IncentiveSignalDto
    {
        public string FirmaAdi { get; set; } = string.Empty;
        public string Il { get; set; } = string.Empty;
        public string Konusu { get; set; } = string.Empty; // Örn: İMALAT - MOBİLYA
        public string Cinsi { get; set; } = string.Empty;  // Örn: TEVSİ / KOMPLE YENİ YATIRIM
    }
}