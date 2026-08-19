using System;
using System.Collections.Generic;

namespace CrmDb.Models;

public partial class ScoreRule
{
    public int RuleId { get; set; }

    public string KriterAdi { get; set; } = null!;

    public string? Aciklama { get; set; }

    public decimal AgirlikPuani { get; set; }

    public bool AktifMi { get; set; }

    public DateTime GuncellemeTarihi { get; set; }
}
