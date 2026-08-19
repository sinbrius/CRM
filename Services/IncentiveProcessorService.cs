using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmDb.Models;
using Microsoft.EntityFrameworkCore;

namespace CrmDb.Services
{
    public class IncentiveProcessorService
    {
        private readonly CrmDbContext _dbContext;

        public IncentiveProcessorService(CrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Metot artık sadece sayı değil, detaylı teşhis mesajı dönüyor
        public async Task<string> ProcessIncentiveCsvAsync(Stream csvStream)
        {
            var extractedCompanies = new List<string>();
            int totalLinesRead = 0;
            int kayseriRowCount = 0;

            using (var reader = new StreamReader(csvStream, Encoding.UTF8))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    totalLinesRead++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Hem virgül hem noktalı virgül ayrımını destekle
                    char delimiter = line.Contains(';') ? ';' : ',';
                    
                    // Bütün tırnak işaretlerini (") temizleyip kolonlara ayırıyoruz
                    var columns = line.Replace("\"", "").Split(delimiter);

                    // Satırın herhangi bir sütununda KAYSERİ var mı? (Tırnaksız temiz kontrol)
                    bool isKayseriRow = columns.Any(c => c.Trim().Equals("KAYSERİ", StringComparison.OrdinalIgnoreCase));

                    if (isKayseriRow)
                    {
                        kayseriRowCount++;

                        // Şirket ismi olabilecek sütunu yakala (İçinde SANAYİ, TİCARET, ŞİRKETİ, A.Ş., LTD geçen sütun)
                        string firmaName = columns.FirstOrDefault(c => 
                            c.Contains("SANAYİ") || c.Contains("TİCARET") || 
                            c.Contains("LİMİTED") || c.Contains("ŞİRKETİ") || 
                            c.Contains("A.Ş") || c.Contains("LTD") || c.Contains("ANONİM"))?.Trim() ?? "";

                        if (!string.IsNullOrWhiteSpace(firmaName) && firmaName.Length > 3)
                        {
                            extractedCompanies.Add(firmaName);
                        }
                    }
                }
            }

            if (kayseriRowCount == 0)
            {
                return $"⚠️ CSV okundu ({totalLinesRead} satır) ama içinde 'KAYSERİ' geçen hiç satır bulunamadı. Dosyayı indirirken Kayseri filtresi uyguladığınızdan veya doğru CSV formatında olduğundan emin olun.";
            }

            if (!extractedCompanies.Any())
            {
                return $"⚠️ Kayseri'ye ait {kayseriRowCount} satır bulundu ama şirket ünvanları ayıklanamadı.";
            }

            // 🔄 VERİTABANI EŞLEŞTİRME
            int matchedCount = 0;
            var dbCompanies = await _dbContext.Companies.ToListAsync();
            List<string> matchedNames = new List<string>();

            foreach (var csvFirma in extractedCompanies)
            {
                string cleanCsvFirma = csvFirma.ToLower();
                string firstWord = cleanCsvFirma.Split(' ')[0]; // Örn: "Oğuz"

                var matchedCompany = dbCompanies.FirstOrDefault(c => 
                    c.UnvanResmi.ToLower().Contains(cleanCsvFirma) || 
                    cleanCsvFirma.Contains(c.UnvanResmi.ToLower()) ||
                    (firstWord.Length > 3 && c.UnvanResmi.ToLower().StartsWith(firstWord)));

                if (matchedCompany != null)
                {
                    matchedCompany.Skor += 30.00m;
                    if (matchedCompany.Skor > 100.00m) matchedCompany.Skor = 100.00m;
                    matchedCompany.SonGuncelleme = DateTime.Now;

                    _dbContext.Activities.Add(new Activity
                    {
                        CompanyId = matchedCompany.CompanyId,
                        UserId = 1,
                        Tip = "Yatırım Teşvik Sinyali",
                        Icerik = $"Yatırım Teşviki Tespiti: {csvFirma} (+30 Puan)",
                        Tarih = DateTime.Now
                    });

                    matchedNames.Add(matchedCompany.UnvanResmi);
                    matchedCount++;
                }
            }

            await _dbContext.SaveChangesAsync();

            if (matchedCount == 0)
            {
                return $"ℹ️ CSV'den {kayseriRowCount} adet Kayseri firması çekildi ({string.Join(", ", extractedCompanies.Take(3))}...) ancak veritabanındaki 425 firma ile isimleri örtüşmedi.";
            }

            return $"🎉 Başarılı! CSV'deki {kayseriRowCount} Kayseri firmasından {matchedCount} tanesi veritabanıyla eşleşti ve skorları +30 yükseltildi! (Eşleşenler: {string.Join(", ", matchedNames)})";
        }
    }
}