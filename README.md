# 🏢 B2B Müşteri İstihbaratı ve CRM Takip Platformu

> Açık veri kaynaklarından elde edilen verileri işleyen, kurumsal potansiyel müşterileri önceliklendiren (Lead Scoring) ve web scraping ile zenginleştirilmiş bilgi kartları sunan .NET 8 Blazor CRM sistemi.

---

## 🚀 Canlı Önizleme (Demo)

![CRM Demo](assets/demo.gif)

---

## ✨ Temel Özellikler

* **Müşteri Önceliklendirme & Skorlama (Lead Ranking):** Dış sinyal ve teşvik verilerine göre firmaların otomatik puanlanması ve öncelik sırasına göre listelenmesi.
* **Akıllı Web Kazıyıcı (Web Scraper):** Kurumsal web siteleri ve OSB dizinlerinden firma logolarını ve "Hakkımızda / Kurumsal" tanıtım özetlerini asenkron olarak toplayan C# servisi.
* **Etkileşimli Bilgi Kartı (Modal / Pop-up):** Firma detaylarını, çekilen logoyu ve kurumsal özeti anlık gösteren ve gerektiğinde canlı taramayı tetikleyen modal mimarisi.
* **Gelişmiş Çoklu Filtreleme:** Firma unvanı, şehir, sektör kodu ve satış sürecine göre eşzamanlı ve esnek arama paneli.
* **Aktivite ve Not Takip Sistemi:** Satış ekibinin görüşme türlerine (Arama, E-posta, Ziyaret) göre tarihçeli not düşebildiği dinamik takip modülü.
* **Veritabanı Entegrasyonu & Null-Safety:** EF Core ve SQL Server üzerinde optimize edilmiş, `AsNoTracking` ve `NULL` güvenliği sağlanmış veri modeli.

---

## 🛠️ Kullanılan Teknolojiler ve Kütüphaneler

* **Backend / UI:** .NET 8, C#, Blazor Server (`InteractiveServer`)
* **Veritabanı & ORM:** Microsoft SQL Server, Entity Framework Core
* **Web Scraping & Parsing:** HtmlAgilityPack, Regex, System.Net.Http
* **Arayüz Tasarımı:** Bootstrap 5, Custom CSS

---

## 💻 Kurulum ve Çalıştırma

1. **Depoyu Klonlayın:**
   ```bash
   git clone [https://github.com/sinbrius/CRM.git](https://github.com/sinbrius/CRM.git)
   cd CRM
