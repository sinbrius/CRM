using System;
using System.Collections.Generic;

namespace CrmDb.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string UnvanResmi { get; set; } = null!;

    public string? VergiNo { get; set; }
    public string? Telefon { get; set; }
    public string? Eposta { get; set; }
    public string? WebSitesi { get; set; }

    public string? SektorKodu { get; set; }

    public string Sehir { get; set; } = null!;

    public string? Adres { get; set; }

    public string? TahminiBuyukluk { get; set; }

    public decimal Skor { get; set; }

    public string Durum { get; set; } = null!;
    public string? BilgiKarti { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }


    public DateTime OlusturmaTarihi { get; set; }

    public DateTime SonGuncelleme { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual ICollection<ContactPerson> ContactPeople { get; set; } = new List<ContactPerson>();

    public virtual ICollection<MatchLog> MatchLogs { get; set; } = new List<MatchLog>();

    public virtual ICollection<ScoreHistory> ScoreHistories { get; set; } = new List<ScoreHistory>();

    public virtual ICollection<SourceRecord> SourceRecords { get; set; } = new List<SourceRecord>();
}
