using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CrmDb.Models;
using Microsoft.EntityFrameworkCore;

namespace CrmDb.Services
{
    public class CompanyIngestionService
    {
        private readonly CrmDbContext _dbContext;

        public CompanyIngestionService(CrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> ProcessRawRecordsAsync()
        {
            // İşlenmemiş veya güncellenmesi gereken kayıtları çekiyoruz
            var unprocessedRecords = await _dbContext.SourceRecords
                .Where(sr => sr.CompanyId == null || sr.EslestirmeDurumu == "Ham Kayıt" || sr.EslestirmeDurumu == "Yeni Kayıt")
                .ToListAsync();

            int processedCount = 0;

            foreach (var record in unprocessedRecords)
            {
                if (string.IsNullOrWhiteSpace(record.HamVeriJson)) continue;

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var rawData = JsonSerializer.Deserialize<CompanyDto>(record.HamVeriJson, options);

                    if (rawData == null || string.IsNullOrWhiteSpace(rawData.FirmaAdi)) continue;

                    string cleanUnvan = rawData.FirmaAdi.Trim();
                    string cleanAdres = rawData.Adres?.Trim() ?? string.Empty;
                    string cleanTel = rawData.Telefon?.Trim() ?? string.Empty;
                    string cleanMail = rawData.Eposta?.Trim() ?? string.Empty;
                    string cleanWeb = rawData.WebSitesi?.Trim() ?? string.Empty;

                    // 1. Firma veritabanında daha önce oluşturulmuş mu kontrol et
                    var existingCompany = await _dbContext.Companies
                        .FirstOrDefaultAsync(c => c.UnvanResmi.ToLower() == cleanUnvan.ToLower());

                    if (existingCompany != null)
                    {
                        // 🔄 VAR OLAN FİRMAYI GÜNCELLE (UPDATE)
                        existingCompany.Telefon = !string.IsNullOrEmpty(cleanTel) ? cleanTel : existingCompany.Telefon;
                        existingCompany.Eposta = !string.IsNullOrEmpty(cleanMail) ? cleanMail : existingCompany.Eposta;
                        existingCompany.WebSitesi = !string.IsNullOrEmpty(cleanWeb) ? cleanWeb : existingCompany.WebSitesi;
                        if (!string.IsNullOrEmpty(cleanAdres)) existingCompany.Adres = cleanAdres;
                        
                        existingCompany.SonGuncelleme = DateTime.Now;

                        // SourceRecord bağlama
                        record.CompanyId = existingCompany.CompanyId;
                        record.EslestirmeDurumu = "Eşleşti (Güncellendi)";

                        // Loglama
                        _dbContext.MatchLogs.Add(new MatchLog
                        {
                            SourceRecordId = record.SourceRecordId,
                            MatchedCompanyId = existingCompany.CompanyId,
                            MatchType = "TitleMatch",
                            SimilarityScore = 100.00m,
                            AppliedThreshold = 90.00m,
                            Decision = "UpdatedExisting",
                            CreatedDate = DateTime.Now
                        });
                    }
                    else
                    {
                        // ➕ YENİ FİRMA EKLE (INSERT)
                        var newCompany = new Company
                        {
                            UnvanResmi = cleanUnvan,
                            Adres = cleanAdres,
                            Telefon = cleanTel,
                            Eposta = cleanMail,
                            WebSitesi = cleanWeb,
                            Sehir = "Kayseri",
                            Durum = "Yeni",
                            OlusturmaTarihi = DateTime.Now,
                            SonGuncelleme = DateTime.Now
                        };

                        _dbContext.Companies.Add(newCompany);
                        await _dbContext.SaveChangesAsync(); // ID oluşması için kaydet

                        record.CompanyId = newCompany.CompanyId;
                        record.EslestirmeDurumu = "Aktarıldı";

                        _dbContext.MatchLogs.Add(new MatchLog
                        {
                            SourceRecordId = record.SourceRecordId,
                            MatchedCompanyId = newCompany.CompanyId,
                            MatchType = "NewEntry",
                            SimilarityScore = 0.00m,
                            AppliedThreshold = 90.00m,
                            Decision = "CreatedAsNew",
                            CreatedDate = DateTime.Now
                        });
                    }

                    processedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kayıt işleme hatası (ID: {record.SourceRecordId}): {ex.Message}");
                }
            }

            await _dbContext.SaveChangesAsync();
            return processedCount;
        }
    }
}