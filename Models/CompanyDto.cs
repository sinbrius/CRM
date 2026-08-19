namespace CrmDb.Models
{
    public class CompanyDto
    {
        public string FirmaAdi { get; set; } = string.Empty;
        public string Sektor { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public string WebSitesi { get; set; } = string.Empty;
        public string FaaliyetDurumu { get; set; } = string.Empty;
        public string KaynakUrl { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string BilgiKarti { get; set; } = string.Empty;
    }
}