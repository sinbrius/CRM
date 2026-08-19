using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace CrmDb.Services
{
    public class WebSiteScraper
    {
        private readonly HttpClient _httpClient;

        public WebSiteScraper()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10); // Yavaş sitelerde kilitlenmemesi için 10 sn timeout
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36");
        }

            public async Task<ScrapedCompanyData> ScrapeCompanyAboutAsync(string websiteUrl)
        {
            var result = new ScrapedCompanyData();
            if (string.IsNullOrWhiteSpace(websiteUrl))
            {
                result.SummaryText = "Web adresi bulunamadı.";
                return result;
            }

            try
            {
                if (!websiteUrl.StartsWith("http://") && !websiteUrl.StartsWith("https://"))
                    websiteUrl = "https://" + websiteUrl.Trim();

                var baseUri = new Uri(websiteUrl);
                var htmlString = await _httpClient.GetStringAsync(websiteUrl);
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlString);

                // 1. Logo Taraması (class veya id'sinde 'logo' geçen img etiketleri)
                var logoNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'logo')]//img | //img[contains(@class,'logo') or contains(@id,'logo')]");
                if (logoNode != null)
                {
                    var src = logoNode.GetAttributeValue("src", "");
                    if (!string.IsNullOrWhiteSpace(src))
                    {
                        result.LogoUrl = new Uri(baseUri, src).AbsoluteUri; // Göreceli (/img/logo.png) yolu mutlak URL yapar
                    }
                }

                // 2. Metin Taraması: Ana sayfada yeterli metin yoksa alt sayfalara bak
                string extractedText = ExtractAboutText(doc);

                if (string.IsNullOrWhiteSpace(extractedText) || extractedText.Length < 60)
                {
                    // Kurumsal veya Hakkımızda linkini bul
                    var aboutLink = doc.DocumentNode.SelectSingleNode("//a[contains(@href,'hakkimizda') or contains(@href,'kurumsal') or contains(@href,'about')]");
                    if (aboutLink != null)
                    {
                        var href = aboutLink.GetAttributeValue("href", "");
                        var subPageUri = new Uri(baseUri, href);
                        var subHtml = await _httpClient.GetStringAsync(subPageUri);
                        var subDoc = new HtmlDocument();
                        subDoc.LoadHtml(subHtml);
                        extractedText = ExtractAboutText(subDoc);
                    }
                }

                result.SummaryText = string.IsNullOrWhiteSpace(extractedText) 
                    ? "Kurumsal tanıtım metni bulunamadı." 
                    : extractedText;

                return result;
            }
            catch (Exception ex)
            {
                result.SummaryText = $"Veri çekilemedi: {ex.Message}";
                return result;
            }
        }

        private string ExtractAboutText(HtmlDocument doc)
        {
            var pNodes = doc.DocumentNode.SelectNodes("//p | //div[contains(@class,'about') or contains(@class,'kurumsal')]");
            if (pNodes == null) return "";

            string combined = "";
            foreach (var p in pNodes)
            {
                string text = Regex.Replace(p.InnerText, @"\s+", " ").Trim();
                if (text.Length > 30 && !text.Contains("cookie") && !text.Contains("çerez"))
                {
                    combined += text + " ";
                    if (combined.Length > 350) break;
                }
            }
            return combined.Trim();
        }
    }
}