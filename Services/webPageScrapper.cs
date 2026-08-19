using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using CrmDb.Models;

namespace CrmDb.Services
{


    public class WebPageScraper
    {
        private const string BaseUrl = "http://www.mimarsinanosb.org.tr/";

        public async Task<List<CompanyDto>> ScrapeCompaniesAsync()
        {
            var scrapedCompanies = new List<CompanyDto>();
            var web = new HtmlWeb();

            return await Task.Run(async () =>
            {
                try
                {
                    // 1. Ana Firmalar / Sektörler Sayfasına Gidiyoruz
                    var mainDoc = await web.LoadFromWebAsync(BaseUrl + "firmalar");

                    // 2. Sitedeki TÜM Sektör Linklerini Dinamik Olarak Buluyoruz (sektor=1, sektor=2, sektor=23 vs.)
                    var sectorNodes = mainDoc.DocumentNode.SelectNodes("//a[contains(@href, 'sektor=')]");

                    if (sectorNodes == null) return scrapedCompanies;

                    // Sektör URL'lerini tekilleştiriyoruz
                    var sectorUrls = sectorNodes
                        .Select(n => n.GetAttributeValue("href", string.Empty))
                        .Where(href => !string.IsNullOrEmpty(href))
                        .Distinct()
                        .ToList();

                    // 3. Her Bir Sektörün İçine Sırayla Giriyoruz
                    foreach (var sectorHref in sectorUrls)
                    {
                        string fullSectorUrl = sectorHref.StartsWith("http") ? sectorHref : BaseUrl + sectorHref.TrimStart('/');

                        try
                        {
                            var sectorDoc = await web.LoadFromWebAsync(fullSectorUrl);

                            // Sektör sayfasındaki `.html` uzantılı firma detay linklerini yakalıyoruz
                            var allLinks = sectorDoc.DocumentNode.SelectNodes("//a[@href]");
                            if (allLinks == null) continue;

                            var companyLinks = allLinks
                                .Select(n => n.GetAttributeValue("href", string.Empty))
                                .Where(href => href.Contains("/firmalar/") && href.EndsWith(".html"))
                                .Distinct()
                                 // HER SEKTÖRDEN İLK 3 FİRMAYI ÇEKER (Test hızlansın diye. Tümü için .Take(3) kaldırılabilir)
                                .ToList();

                            // 4. Firma Detay Sayfalarından Bilgileri Çekiyoruz
                            foreach (var link in companyLinks)
                            {
                                string detailUrl = link.StartsWith("http") ? link : BaseUrl + link.TrimStart('/');

                                try
                                {
                                    var detailDoc = await web.LoadFromWebAsync(detailUrl);

                                    string firmaAdi = GetTableValue(detailDoc, "Firma Adı");
                                    string adres = GetTableValue(detailDoc, "Adres");
                                    string telefon = GetTableValue(detailDoc, "Telefon");
                                    string eposta = GetTableValue(detailDoc, "Eposta");
                                    string faaliyet = GetTableValue(detailDoc, "Faaliyet Durumu");
                                    string website = GetTableValue(detailDoc, "Web ");

                                    var company = new CompanyDto
                                    {
                                        KaynakUrl = detailUrl,
                                        FirmaAdi = firmaAdi,
                                        Adres = adres,
                                        Telefon = telefon,
                                        Eposta = eposta,
                                        FaaliyetDurumu = faaliyet,
                                        WebSitesi = website
                                    };

                                    if (!string.IsNullOrEmpty(company.FirmaAdi))
                                    {
                                        scrapedCompanies.Add(company);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Firma detay hatası ({detailUrl}): {ex.Message}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Sektör taranırken hata ({fullSectorUrl}): {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Scraper Genel Hatası: {ex.Message}");
                }

                return scrapedCompanies;
            });
        }

        private string GetTableValue(HtmlDocument doc, string labelName)
        {
            var node = doc.DocumentNode.SelectSingleNode($"//*[contains(text(), '{labelName}')]/following-sibling::*")
                       ?? doc.DocumentNode.SelectSingleNode($"//td[contains(., '{labelName}')]/following-sibling::td");

            return node?.InnerText?.Replace(":", "")?.Trim() ?? string.Empty;
        }
    }
}